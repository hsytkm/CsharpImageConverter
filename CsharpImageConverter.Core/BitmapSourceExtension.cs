using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CsharpImageConverter.Core
{
    public static class BitmapSourceExtension
    {
        /// <summary>BitmapSourceに異常がないかチェックします</summary>
        public static bool IsValid(this BitmapSource bitmap)
        {
            if (bitmap.PixelWidth == 0 || bitmap.PixelHeight == 0) return false;
            return true;
        }

        /// <summary>BitmapSourceに異常がないかチェックします</summary>
        public static bool IsInvalid(this BitmapSource bitmap) => !IsValid(bitmap);

        /// <summary>1PixelのByte数を取得します</summary>
        public static int GetBytesPerPixel(this BitmapSource bitmap)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");
            return Ceiling(bitmap.Format.BitsPerPixel, 8);

            static int Ceiling(int value, int div) => (value + (div - 1)) / div;
        }

        #region FromFile
        /// <summary>引数PATHのファイルを画像として読み出します</summary>
        public static BitmapImage FromFile(string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException(imagePath);

            static BitmapImage ToBitmapImage(Stream stream)
            {
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CreateOptions = BitmapCreateOptions.None;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = stream;
                bi.EndInit();
                bi.Freeze();

                if (bi.Width == 1 && bi.Height == 1) throw new OutOfMemoryException();
                return bi;
            }

            using var stream = File.Open(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return ToBitmapImage(stream);

            //return new BitmapImage(new Uri(imagePath));  これでも読めるがファイルがロックされる
        }
        #endregion

        #region ToFile
        /// <summary>画像をファイルに保存します</summary>
        private static void ToFileImpl(BitmapSource bitmap, string savePath, BitmapEncoder encoder)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (File.Exists(savePath)) throw new ArgumentException("Save file is already exist.");

            using var fs = new FileStream(savePath, FileMode.Create);
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(fs);
        }

        /// <summary>画像をpngファイルに保存します</summary>
        public static void ToPngFile(this BitmapSource bitmap, string savePath)
            => ToFileImpl(bitmap, savePath, new PngBitmapEncoder());

        /// <summary>画像をbmpファイルに保存します</summary>
        public static void ToBmpFile(this BitmapSource bitmap, string savePath)
            => ToFileImpl(bitmap, savePath, new BmpBitmapEncoder());

        /// <summary>画像をjpgファイルに保存します</summary>
        public static void ToJpegFile(this BitmapSource bitmap, string savePath)
            => ToFileImpl(bitmap, savePath, new JpegBitmapEncoder());

        /// <summary>画像をtiffファイルに保存します</summary>
        public static void ToTiffFile(this BitmapSource bitmap, string savePath)
            => ToFileImpl(bitmap, savePath, new TiffBitmapEncoder());

        /// <summary>保存ファイルPATHの拡張子に応じた画像を保存します</summary>
        public static void ToImageFile(this BitmapSource bitmap, string savePath)
        {
            var extension = Path.GetExtension(savePath);
            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                    ToJpegFile(bitmap, savePath);
                    break;
                case ".bmp":
                    ToBmpFile(bitmap, savePath);
                    break;
                case ".png":
                    ToPngFile(bitmap, savePath);
                    break;
                case ".tif":
                case ".tiff":
                    ToTiffFile(bitmap, savePath);
                    break;
                default:
                    throw new NotSupportedException(extension);
            }
        }
        #endregion

        #region ReadPixels
        /// <summary>指定エリアの画素平均値を取得します(ヒープ確保＋画素値コピーが発生し遅いです…)</summary>
        public static IReadOnlyCollection<double> GetChannelsAverage(this BitmapSource bitmap, in Int32Rect rect)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (rect.Width * rect.Height == 0) throw new ArgumentException("Area is zero.");
            if (bitmap.PixelWidth < rect.X + rect.Width) throw new ArgumentException("Width over.");
            if (bitmap.PixelHeight < rect.Y + rect.Height) throw new ArgumentException("Height over.");

            var bytesPerPixel = bitmap.GetBytesPerPixel();
            Span<ulong> sumChannels = stackalloc ulong[bytesPerPixel];

            // 1行ずつメモリに読み出して処理する(ヒープ使用量の削減)
            var bs = new byte[rect.Width * bytesPerPixel];
            var rect1Line = new Int32Rect(rect.X, y: 0, rect.Width, height: 1);

            unsafe
            {
                fixed (byte* head = bs)
                {
                    var tail = head + bs.Length;

                    for (var y = rect.Y; y < (rect.Y + rect.Height); ++y)
                    {
                        rect1Line.Y = y;
                        bitmap.CopyPixels(rect1Line, bs, bs.Length, 0);

                        for (var ptr = head; ptr < tail; ptr += bytesPerPixel)
                        {
                            for (var c = 0; c < bytesPerPixel; ++c)
                            {
                                sumChannels[c] += *(ptr + c);
                            }
                        }
                    }
                }
            }

            var aveChannels = new double[sumChannels.Length];
            var count = (double)(rect.Width * rect.Height);
            for (var i = 0; i < aveChannels.Length; ++i)
            {
                aveChannels[i] = sumChannels[i] / count;
            }
            return aveChannels;
        }

        /// <summary>画面全体の画素平均値を取得します</summary>
        public static IEnumerable<double> GetChannelsAverage(this BitmapSource bitmap)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");

            var fullRect = new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            return GetChannelsAverage(bitmap, fullRect);
        }
        #endregion

        #region ToWriteableBitmap
        /// <summary>System.Windows.Media.Imaging.WriteableBitmap の画素値を更新します(遅いです)</summary>
        public static void CopyToWriteableBitmap(this BitmapSource bitmapSource, WriteableBitmap writeableBitmap)
        {
            if (bitmapSource.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (writeableBitmap.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (writeableBitmap.PixelWidth != bitmapSource.PixelWidth) throw new ArgumentException("Different Width");
            if (writeableBitmap.PixelHeight != bitmapSource.PixelHeight) throw new ArgumentException("Different Height");
            if (writeableBitmap.GetBytesPerPixel() != bitmapSource.GetBytesPerPixel()) throw new ArgumentException("Different BytesPerPixel");

            int srcStride = bitmapSource.PixelWidth * bitmapSource.GetBytesPerPixel();
            var fullRect = new Int32Rect(0, 0, bitmapSource.PixelWidth, bitmapSource.PixelHeight);

            try
            {
                writeableBitmap.Lock();

                bitmapSource.CopyPixels(fullRect,
                    writeableBitmap.BackBuffer,
                    writeableBitmap.BackBufferStride * writeableBitmap.PixelHeight,
                    srcStride);
            }
            finally
            {
                writeableBitmap.AddDirtyRect(fullRect);
                writeableBitmap.Unlock();
            }
            //writeableBitmap.Freeze();
        }

        /// <summary>System.Windows.Media.Imaging.WriteableBitmap を新たに作成します</summary>
        public static WriteableBitmap ToWriteableBitmap(this BitmapSource bitmapSource)
        {
            if (bitmapSource.IsInvalid()) throw new ArgumentException("Invalid Image");

            var writeableBitmap = new WriteableBitmap(bitmapSource);

            //writeableBitmap.Freeze();
            return writeableBitmap;
        }
        #endregion

        #region ToDrawingBitmap
        /// <summary>System.Drawing.Bitmap に変換します</summary>
        public static System.Drawing.Bitmap ToDrawingBitmap(this BitmapSource bitmapSource)
        {
            if (bitmapSource.IsInvalid()) throw new ArgumentException("Invalid Image");

            var bitmap = new System.Drawing.Bitmap(bitmapSource.PixelWidth, bitmapSource.PixelHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

            try
            {
                bitmapSource.CopyPixels(Int32Rect.Empty, bitmapData.Scan0,
                    bitmapData.Height * bitmapData.Stride, bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
            return bitmap;
        }
        #endregion

        #region ToImagePixels
        /// <summary>ImagePixels に画素値をコピーします</summary>
        public static void CopyToImagePixels(this BitmapSource bitmap, ref ImagePixels pixels)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Bitmap");
            if (pixels.IsInvalid) throw new ArgumentException("Invalid Pixels");
            if (bitmap.PixelWidth != pixels.Width) throw new ArgumentException("Different Width");
            if (bitmap.PixelHeight != pixels.Height) throw new ArgumentException("Different Height");
            if (bitmap.GetBytesPerPixel() < pixels.BytesPerPixel) throw new ArgumentException("Invalid BytesPerPixel");

            var bytesPerPixel = bitmap.GetBytesPerPixel();

            // 1行ずつメモリに読み出して処理する(ヒープ使用量の削減)
            var bs = new byte[bitmap.PixelWidth * bytesPerPixel];
            var rect1Line = new Int32Rect(0, 0, bitmap.PixelWidth, height: 1);

            unsafe
            {
                var destHead = (byte*)pixels.PixelsPtr;

                fixed (byte* head = bs)
                {
                    var tail = head + bs.Length;

                    for (var y = 0; y < bitmap.PixelHeight; ++y)
                    {
                        rect1Line.Y = y;
                        bitmap.CopyPixels(rect1Line, bs, bs.Length, 0);

                        var dest = (Pixel3ch*)(destHead + y * pixels.Stride);
                        for (var ptr = head; ptr < tail; ptr += bytesPerPixel)
                        {
                            *(dest++) = *((Pixel3ch*)ptr);
                        }
                    }
                }
            }
        }

        /// <summary>ImagePixelsContainer を作成して返します</summary>
        public static ImagePixelsContainer ToImagePixelsContainer(this BitmapSource bitmap)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");

            var container = new ImagePixelsContainer(bitmap.PixelWidth, bitmap.PixelHeight);
            var pixels = container.Pixels;
            CopyToImagePixels(bitmap, ref pixels);

            return container;
        }
        #endregion

        #region ToImageSharpBgr24
        /// <summary>SixLabors.ImageSharp.Image に変換します</summary>
        public static SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Bgr24> ToImageSharpBgr24(this BitmapSource bitmap)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid ImagePixels");
            if (bitmap.GetBytesPerPixel() < 3) throw new NotSupportedException("Invalid BytesPerPixel");

            var image = new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Bgr24>(bitmap.PixelWidth, bitmap.PixelHeight);
            var height = image.Height;
            var width = image.Width;

            // 1行ずつメモリに読み出して処理する(ヒープ使用量の削減)
            var srcBytesPerPixel = bitmap.GetBytesPerPixel();
            var bs = new byte[bitmap.PixelWidth * srcBytesPerPixel];
            var rect1Line = new Int32Rect(0, 0, bitmap.PixelWidth, height: 1);

            unsafe
            {
                fixed (byte* head = bs)
                {
                    for (var y = 0; y < height; ++y)
                    {
                        rect1Line.Y = y;
                        bitmap.CopyPixels(rect1Line, bs, bs.Length, 0);

                        var pixelRowSpan = image.GetPixelRowSpan(y);
                        var src = head;

                        for (var x = 0; x < width; ++x, src += srcBytesPerPixel)
                        {
                            pixelRowSpan[x] = *(SixLabors.ImageSharp.PixelFormats.Bgr24*)src;
                        }
                    }
                }
            }
            return image;
        }
        #endregion

    }
}
