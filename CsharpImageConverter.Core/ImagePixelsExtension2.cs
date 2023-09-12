using System;
using System.IO;
using System.Runtime.CompilerServices;
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
        }

        private static byte[] GetBitmapBinary(in ImagePixels pixels)
        {
            (int width, int height, int srcStride) = (pixels.Width, pixels.Height, pixels.Stride);

            // 画像に必要なメモリを確保します(画素値は書き込まれていません)
            var destHeader = new BitmapHeader(width, height, pixels.BitsPerPixel);
            byte[] destBuffer = destHeader.GetImageMemory();

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

#pragma warning disable IDE0049
        // http://www.umekkii.jp/data/computer/file_format/bitmap.cgi
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private readonly struct BitmapHeader
        {
            private const Int32 _pixelPerMeter = 3780;    // pixel/meter (96dpi / 2.54cm * 100m)

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

            // if color palette exists
            // byte[] ColorPalette;

            public BitmapHeader(int width, int height, int bitsPerPixel)
            {
                int fileHeaderSize = 14;
                int infoHeaderSize = 40;
                int colorPaletteSize = GetColorPaletteSize(bitsPerPixel);
                int totalHeaderSize = fileHeaderSize + infoHeaderSize + colorPaletteSize;
                int imageSize = GetImageStride(width, bitsPerPixel) * height;

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

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int GetImageStride(int width, int bitsPerPixel)
            {
                int bytesPerPixel = ceiling(bitsPerPixel, 8);
                return ceiling(width * bytesPerPixel, 4) * 4;   // strideは4の倍数

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                static int ceiling(int value, int align) => (value + (align - 1)) / align;
            }

            // 8bitグレー画像をWindowsで表示するための対応(カラーパレットの埋め込み)
            private static int GetColorPaletteSize(int bitsPerPixel)
                => bitsPerPixel == 8 ? 256 * sizeof(UInt32) : 0;
            private void WriteColorPalette(Span<byte> headSpan)
            {
                int paletteSize = GetColorPaletteSize(BitCount);
                if (paletteSize > 0)
                {
                    Span<byte> bytePaletteSpan = headSpan[(OffsetBytes - paletteSize)..OffsetBytes];
                    Span<UInt32> uintPaletteSpan = MemoryMarshal.Cast<byte, UInt32>(bytePaletteSpan);

                    for (int b = 0; b < uintPaletteSpan.Length; b++)
                        uintPaletteSpan[b] = ((((UInt32)b) << 16) | (((UInt32)b) << 8) | (UInt32)b);
                }
            }

            // 対象のメモリ領域に BitmapHeader を書き込みます
            private void WriteHeader(Span<byte> headSpan)
            {
                if (headSpan.Length < FileSize)
                    throw new ArgumentException("Allocated memory size is short.");

                // bufferにheaderを書き込む
                UnsafeExtensions.CopyStructToArray(this, headSpan);

                // カラーパレットが必要な場合は追加で書き込みます
                WriteColorPalette(headSpan);
            }

            /// <summary>
            /// 画像に必要なメモリを確保してヘッダ部のみを書き込んで返却します。(画素値は書き込まれません)
            /// </summary>
            /// <returns></returns>
            internal byte[] GetImageMemory()
            {
                var bs = new byte[FileSize];

                // メモリにheaderを書き込みます
                WriteHeader(bs.AsSpan());

                return bs;
            }
        }
#pragma warning restore IDE0049
    }
}
