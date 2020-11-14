using System;
using System.Collections.Generic;

namespace CsharpImageConverter.App.Models
{
    static class ImagePixelsExtension
    {
        private const double _dpi = 96.0;

        #region ReadPixels
        /// <summary>指定エリアの画素平均値を取得します</summary>
        public static IReadOnlyCollection<double> GetChannelsAverage(in this ImagePixels pixels, int rectX, int rectY, int rectWidth, int rectHeight)
        {
            if (pixels.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (rectWidth * rectHeight == 0) throw new ArgumentException("Area is zero.");
            if (pixels.Width < rectX + rectWidth) throw new ArgumentException("Width over.");
            if (pixels.Height < rectY + rectHeight) throw new ArgumentException("Height over.");

            var bytesPerPixel = pixels.BytesPerPixel;
            Span<ulong> sumChannels = stackalloc ulong[bytesPerPixel];

            unsafe
            {
                var stride = pixels.Stride;
                var rowHead = (byte*)pixels.PixelsPtr + (rectY * stride);
                var rowTail = rowHead + (rectHeight * stride);
                var columnLength = rectWidth * bytesPerPixel;

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

            var aveChannels = new double[sumChannels.Length];
            var count = (double)(rectWidth * rectHeight);

            for (var i = 0; i < aveChannels.Length; ++i)
            {
                aveChannels[i] = sumChannels[i] / count;
            }
            return aveChannels;
        }

        /// <summary>画面全体の画素平均値を取得します</summary>
        public static IEnumerable<double> GetChannelsAverage(in this ImagePixels pixels)
        {
            if (pixels.IsInvalid()) throw new ArgumentException("Invalid Image");
            return GetChannelsAverage(pixels, 0, 0, pixels.Width, pixels.Height);
        }
        #endregion

        #region ToDrawingBitmap

        /// <summary>System.Drawing.Bitmap に変換します</summary>
        public static System.Drawing.Bitmap ToDrawingBitmap(this in ImagePixels pixels)
        {
            if (pixels.IsInvalid()) throw new ArgumentException("Invalid ImagePixels");
            if (pixels.BytesPerPixel != 3) throw new NotSupportedException("Invalid BytesPerPixel");

            var bitmap = new System.Drawing.Bitmap(pixels.Width, pixels.Height,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size), 
                System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

            try
            {
                if (bitmapData.Stride == pixels.Stride)
                {
                    // strideが一致していたらメモリを丸コピー
                    UnsafeExtensions.MemCopy(bitmapData.Scan0, pixels.PixelsPtr, pixels.AllocSize);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }
        #endregion

        #region ToBitmapSource
        /// <summary>System.Windows.Media.Imaging.BitmapSource に変換します</summary>
        public static System.Windows.Media.Imaging.BitmapSource ToBitmapSource(this in ImagePixels pixels)
        {
            if (pixels.IsInvalid()) throw new ArgumentException("Invalid ImagePixels");
            if (pixels.BytesPerPixel != 3) throw new NotSupportedException("Invalid BytesPerPixel");

            var bitmapSource = System.Windows.Media.Imaging.BitmapSource.Create(
                pixels.Width, pixels.Height, _dpi, _dpi,
                System.Windows.Media.PixelFormats.Bgr24, null,
                pixels.PixelsPtr, pixels.Height * pixels.Stride, pixels.Stride);

            bitmapSource.Freeze();
            return bitmapSource;
        }
        #endregion

        #region ToWriteableBitmap
        /// <summary>System.Windows.Media.Imaging.WriteableBitmap の画素値を更新します(遅いです)</summary>
        public static void CopyToWriteableBitmap(this in ImagePixels pixels, System.Windows.Media.Imaging.WriteableBitmap writeableBitmap)
        {
            if (pixels.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (writeableBitmap.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (writeableBitmap.PixelWidth != pixels.Width) throw new ArgumentException("Different Width");
            if (writeableBitmap.PixelHeight != pixels.Height) throw new ArgumentException("Different Height");
            if (writeableBitmap.GetBytesPerPixel() != pixels.BytesPerPixel) throw new ArgumentException("Different BytesPerPixel");

            writeableBitmap.WritePixels(
                new System.Windows.Int32Rect(0, 0, pixels.Width, pixels.Height),
                pixels.PixelsPtr, pixels.AllocSize, pixels.Stride);

            //writeableBitmap.Freeze();
        }

        /// <summary>System.Windows.Media.Imaging.WriteableBitmap に変換します</summary>
        public static System.Windows.Media.Imaging.WriteableBitmap ToWriteableBitmap(this in ImagePixels pixels)
        {
            if (pixels.IsInvalid()) throw new ArgumentException("Invalid ImagePixels");
            if (pixels.BytesPerPixel != 3) throw new NotSupportedException("Invalid BytesPerPixel");

            var writeableBitmap = new System.Windows.Media.Imaging.WriteableBitmap(
                pixels.Width, pixels.Height,
                _dpi, _dpi, System.Windows.Media.PixelFormats.Bgr24, null);

            writeableBitmap.WritePixels(
                new System.Windows.Int32Rect(0, 0, pixels.Width, pixels.Height),
                pixels.PixelsPtr, pixels.AllocSize, pixels.Stride);

            //writeableBitmap.Freeze();
            return writeableBitmap;
        }
        #endregion

        #region ToImageSharpBgr24
        /// <summary>SixLabors.ImageSharp.Image に変換します</summary>
        public static SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Bgr24> ToImageSharpBgr24(this in ImagePixels pixels)
        {
            if (pixels.IsInvalid()) throw new ArgumentException("Invalid ImagePixels");
            if (pixels.BytesPerPixel != 3) throw new NotSupportedException("Invalid BytesPerPixel");

            var image = new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Bgr24>(pixels.Width, pixels.Height);
            unsafe
            {
                var srcPtr = pixels.PixelsPtr;
                var srcStride = pixels.Stride;
                var height = image.Height;
                var width = image.Width;

                for (var y = 0; y < height; ++y, srcPtr += srcStride)
                {
                    var pixelRowSpan = image.GetPixelRowSpan(y);
                    var src = (SixLabors.ImageSharp.PixelFormats.Bgr24*)srcPtr;

                    for (var x = 0; x < width; ++x)
                    {
                        pixelRowSpan[x] = *(src++);
                    }
                }
            }
            return image;
        }
        #endregion

    }
}
