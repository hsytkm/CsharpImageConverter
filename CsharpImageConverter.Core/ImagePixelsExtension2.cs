using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CsharpImageConverter.Core
{
    public static class ImagePixelsExtensions2
    {
        /// <summary>画像をbmpファイルに保存します</summary>
        public static void ToBmpFile(this in ImagePixels pixels, string savePath)
        {
            if (pixels.IsInvalid) throw new ArgumentException("Invalid ImagePixels");

            var bs = GetBitmapBinary(pixels);
            using var ms = new MemoryStream(bs);
            ms.Seek(0, SeekOrigin.Begin);

            using var fs = new FileStream(savePath, FileMode.Create);
            fs.Seek(0, SeekOrigin.Begin);

            ms.WriteTo(fs);

            static byte[] GetBitmapBinary(in ImagePixels pixels)
            {
                var width = pixels.Width;
                var height = pixels.Height;
                var srcStride = pixels.Stride;

                var destHeader = new BitmapHeader(width, height, pixels.BitsPerPixel);
                var destBuffer = new byte[destHeader.FileSize];

                // bufferにheaderを書き込む
                UnsafeExtensions.CopyStructToArray(destHeader, destBuffer);

                // 画素は左下から右上に向かって記録する
                unsafe
                {
                    var srcHead = (byte*)pixels.PixelsPtr.ToPointer();
                    fixed (byte* pointer = destBuffer)
                    {
                        var destHead = pointer + destHeader.OffsetBytes;
                        var destStride = destHeader.ImageStride;
                        System.Diagnostics.Debug.Assert(srcStride <= destStride);

                        for (var y = 0; y < height; ++y)
                        {
                            var src = srcHead + (height - 1 - y) * srcStride;
                            var dest = destHead + (y * destStride);
                            UnsafeExtensions.MemCopy(dest, src, srcStride);
                        }
                    }
                }
                return destBuffer;
            }
        }
    }

    // http://www.umekkii.jp/data/computer/file_format/bitmap.cgi
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal readonly struct BitmapHeader
    {
        // Bitmap File Header
        public readonly Int16 FileType;
        public readonly Int32 FileSize;
        public readonly Int16 Reserved1;
        public readonly Int16 Reserved2;
        public readonly Int32 OffsetBytes;

        // Bitmap Information Header
        public readonly Int32 InfoSize;
        public readonly Int32 Width;
        public readonly Int32 Height;
        public readonly Int16 Planes;
        public readonly Int16 BitCount;
        public readonly Int32 Compression;
        public readonly Int32 SizeImage;
        public readonly Int32 XPixPerMete;
        public readonly Int32 YPixPerMete;
        public readonly Int32 ClrUsed;
        public readonly Int32 CirImportant;

        private const Int32 _pixelPerMeter = 3780;    // pixel/meter (96dpi / 2.54cm * 100m)

        public BitmapHeader(int width, int height, int bitsPerPixel)
        {
            var fileHeaderSize = 14;
            var infoHeaderSize = 40;
            var totalHeaderSize = fileHeaderSize + infoHeaderSize;
            var imageSize = GetImageSize(width, height, bitsPerPixel);

            FileType = 0x4d42;  // 'B','M'
            FileSize = totalHeaderSize + imageSize;
            Reserved1 = 0;
            Reserved2 = 0;
            OffsetBytes = totalHeaderSize;

            InfoSize = infoHeaderSize;
            Width = width;
            Height = height;
            Planes = 1;
            BitCount = (Int16)bitsPerPixel;
            Compression = 0;
            SizeImage = 0;      // 無圧縮の場合、ファイルサイズでなく 0 を設定するみたい
            XPixPerMete = _pixelPerMeter;
            YPixPerMete = _pixelPerMeter;
            ClrUsed = 0;
            CirImportant = 0;
        }

        public int ImageStride => GetImageStride(Width, BitCount);

        private static int Ceiling(int value, int align) => (value + (align - 1)) / align;

        private static int GetImageStride(int width, int bitsPerPixel)
        {
            var bytesPerPixel = Ceiling(bitsPerPixel, 8);
            return Ceiling(width * bytesPerPixel, 4) * 4;   // strideは4の倍数
        }

        private static int GetImageSize(int width, int height, int bitsPerPixel)
            => GetImageStride(width, bitsPerPixel) * height;
    }
}
