using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace CsharpImageConverter.App.Models
{
    static class DrawingBitmapExtension
    {
        private const double _dpi = 96.0;

        /// <summary>Bitmapに異常がないかチェックします</summary>
        public static bool IsValid(this Bitmap bitmap)
        {
            if (bitmap.Width == 0 || bitmap.Height == 0) return false;
            return true;
        }

        /// <summary>Bitmapに異常がないかチェックします</summary>
        public static bool IsInvalid(this Bitmap bitmap) => !IsValid(bitmap);

        /// <summary>1PixelのByte数を取得します</summary>
        public static int GetBytesPerPixel(this Bitmap bitmap)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");
            return Ceiling(Image.GetPixelFormatSize(bitmap.PixelFormat), 8);

            static int Ceiling(int value, int div) => (value + (div - 1)) / div;
        }

        #region FromFile
        /// <summary>引数PATHのファイルを画像として読み出します</summary>
        public static Bitmap FromFile(string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException(imagePath);
            return new Bitmap(imagePath);
        }
        #endregion

        #region ToFile
        /// <summary>画像をファイルに保存します</summary>
        public static void ToFileBody(Bitmap bitmap, string savePath, ImageFormat format)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (File.Exists(savePath)) throw new ArgumentException("Save file is already exist.");

            bitmap.Save(savePath, format);
        }

        /// <summary>画像をpngファイルに保存します</summary>
        public static void ToPngFile(this Bitmap bitmap, string savePath)
            => ToFileBody(bitmap, savePath, ImageFormat.Png);

        /// <summary>画像をbmpファイルに保存します</summary>
        public static void ToBmpFile(this Bitmap bitmap, string savePath)
            => ToFileBody(bitmap, savePath, ImageFormat.Bmp);

        /// <summary>画像をjpgファイルに保存します</summary>
        public static void ToJpegFile(this Bitmap bitmap, string savePath)
            => ToFileBody(bitmap, savePath, ImageFormat.Jpeg);

        #endregion

        #region ReadPixels
        /// <summary>指定エリアの画素平均値を取得します</summary>
        public static IReadOnlyCollection<double> GetChannelsAverage(this Bitmap bitmap, in Rectangle rect)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (rect.Width * rect.Height == 0) throw new ArgumentException("Area is zero.");
            if (bitmap.Width < rect.X + rect.Width) throw new ArgumentException("Width over.");
            if (bitmap.Height < rect.Y + rect.Height) throw new ArgumentException("Height over.");

            var bytesPerPixel = bitmap.GetBytesPerPixel();
            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
            Span<ulong> sumChannels = stackalloc ulong[bytesPerPixel];

            try
            {
                unsafe
                {
                    var stride = bitmapData.Stride;
                    var rowHead = (byte*)bitmapData.Scan0 + (rect.Y * stride);
                    var rowTail = rowHead + (rect.Height * stride);
                    var columnLength = rect.Width * bytesPerPixel;

                    for (byte* rowPtr = rowHead; rowPtr < rowTail; rowPtr += stride)
                    {
                        for (byte* ptr = rowPtr; ptr < (rowPtr + columnLength); ptr += bytesPerPixel)
                        {
                            for (var c = 0; c < bytesPerPixel; ++c)
                            {
                                sumChannels[c] += *(ptr + c);
                            }
                        }
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
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
        public static IEnumerable<double> GetChannelsAverage(this Bitmap bitmap)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");

            var fullRect = new Rectangle(Point.Empty, bitmap.Size);
            return GetChannelsAverage(bitmap, fullRect);
        }
        #endregion

        #region ToXXX

        /// <summary>System.Windows.Media.Imaging.BitmapSource に変換します</summary>
        public static System.Windows.Media.Imaging.BitmapSource ToBitmapSource1(this Bitmap bitmap)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");

            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);

            var bitmapSource = System.Windows.Media.Imaging.BitmapFrame.Create(ms,
                System.Windows.Media.Imaging.BitmapCreateOptions.None,
                System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);

            bitmapSource.Freeze();
            return bitmapSource;
        }

        /// <summary>System.Windows.Media.Imaging.BitmapSource に変換します</summary>
        public static System.Windows.Media.Imaging.BitmapSource ToBitmapSource2(this Bitmap bitmap)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");

            using var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);

            var bitmapImage = new System.Windows.Media.Imaging.BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.None;
            bitmapImage.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = ms;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>System.Windows.Media.Imaging.BitmapSource に変換します</summary>
        public static System.Windows.Media.Imaging.BitmapSource ToBitmapSourceFast(this Bitmap bitmap)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");

            var hBitmap = bitmap.GetHbitmap();
            try
            {
                var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                bitmapSource.Freeze();
                return bitmapSource;
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        /// <summary>System.Windows.Media.Imaging.WriteableBitmap の画素値を更新します</summary>
        public static System.Windows.Media.Imaging.WriteableBitmap UpdateWriteableBitmap(this Bitmap bitmap, System.Windows.Media.Imaging.WriteableBitmap writeableBitmap)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (writeableBitmap.PixelWidth != bitmap.Width) throw new ArgumentException("Different Width");
            if (writeableBitmap.PixelHeight != bitmap.Height) throw new ArgumentException("Different Height");
            if (writeableBitmap.GetBytesPerPixel() != bitmap.GetBytesPerPixel()) throw new ArgumentException("Different BytesPerPixel");

            var bitmapData = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            try
            {
                writeableBitmap.WritePixels(new System.Windows.Int32Rect(0, 0, bitmap.Width, bitmap.Height),
                    bitmapData.Scan0, bitmapData.Height * bitmapData.Stride, bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
            return writeableBitmap;
        }

        /// <summary>System.Windows.Media.Imaging.WriteableBitmap を新たに作成します</summary>
        public static System.Windows.Media.Imaging.WriteableBitmap ToWriteableBitmap(this Bitmap bitmap)
        {
            if (bitmap.IsInvalid()) throw new ArgumentException("Invalid Image");

            var writeableBitmap = new System.Windows.Media.Imaging.WriteableBitmap(
                bitmap.Width, bitmap.Height, _dpi, _dpi, System.Windows.Media.PixelFormats.Bgr24, null);

            return bitmap.UpdateWriteableBitmap(writeableBitmap);
        }

        #endregion

    }
}
