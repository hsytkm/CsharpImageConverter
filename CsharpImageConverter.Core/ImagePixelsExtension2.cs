using System;
using System.IO;
using System.Runtime.InteropServices;

namespace CsharpImageConverter.Core
{
    public static class ImagePixelsExtensions2
    {
        /// <summary>
        /// 画像をbmpファイルに保存します
        /// 画像サイズによっては正常に動作してない気がするのでオススメしません…
        /// </summary>
        public static void ToBmpFile(this in ImagePixels pixels, string savePath)
        {
            if (pixels.IsInvalid()) throw new ArgumentException("Invalid ImagePixels");

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
                var stride = pixels.Stride;
                var pixelsSize = pixels.AllocSize;

                var header = new BitmapHeader(width, height, pixelsSize, pixels.BytesPerPixel);
                var headerSize = Marshal.SizeOf(header);
                var destBuffer = new byte[headerSize + pixelsSize];

                // bufferにheaderを書き込む
                UnsafeExtensions.CopyStructToArray(header, destBuffer);

                // 画素は左下から右上に向かって記録する
                unsafe
                {
                    var srcHead = (byte*)pixels.PixelsPtr.ToPointer();
                    fixed (byte* pointer = destBuffer)
                    {
                        var destHead = pointer + headerSize;
                        for (var y = 0; y < height; ++y)
                        {
                            var src = srcHead + (height - 1 - y) * stride;
                            var dest = destHead + (y * stride);
                            UnsafeExtensions.MemCopy(dest, src, stride);
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

        public BitmapHeader(int width, int height, int pixelsSize, int bytesPerBits)
        {
            var fileHeaderSize = 14;
            var infoHeaderSize = 40;
            var totalHeaderSize = fileHeaderSize + infoHeaderSize;
            var fileSize = totalHeaderSize + pixelsSize;

            FileType = 0x4d42;  // 'B','M'
            FileSize = fileSize;
            Reserved1 = 0;
            Reserved2 = 0;
            OffsetBytes = totalHeaderSize;

            InfoSize = infoHeaderSize;
            Width = width;
            Height = height;
            Planes = 1;
            BitCount = (Int16)(bytesPerBits * 8);
            Compression = 0;
            SizeImage = pixelsSize;
            XPixPerMete = 0;
            YPixPerMete = 0;
            ClrUsed = 0;
            CirImportant = 0;
        }
    }
}
