using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CsharpImageConverter.App.Models
{
    static class BitmapSourceExtension
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
        private static void ToFileBody(BitmapSource bitmap, string savePath, BitmapEncoder encoder)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (File.Exists(savePath)) throw new ArgumentException("Save file is already exist.");

            using var fs = new FileStream(savePath, FileMode.Create);
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            encoder.Save(fs);
        }

        /// <summary>画像をpngファイルに保存します</summary>
        public static void ToPngFile(this BitmapSource bitmap, string savePath)
            => ToFileBody(bitmap, savePath, new PngBitmapEncoder());

        /// <summary>画像をbmpファイルに保存します</summary>
        public static void ToBmpFile(this BitmapSource bitmap, string savePath)
            => ToFileBody(bitmap, savePath, new BmpBitmapEncoder());

        /// <summary>画像をjpgファイルに保存します</summary>
        public static void ToJpegFile(this BitmapSource bitmap, string savePath)
            => ToFileBody(bitmap, savePath, new JpegBitmapEncoder());

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

        #region ToXXX
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

    }
}
