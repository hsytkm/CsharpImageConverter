using System;
using System.Runtime.InteropServices;

namespace CsharpImageConverter.App.Models
{
    static class ImagePixelsExtension
    {
        private const double _dpi = 96.0;

        #region ToDrawingBitmap

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        private static extern void RtlMoveMemory(IntPtr dest, IntPtr src, [MarshalAs(UnmanagedType.U4)] int length);

        /// <summary>System.Drawing.Bitmap に変換します</summary>
        public static System.Drawing.Bitmap ToDrawingBitmap(this in ImagePixels pixels)
        {
            if (pixels.IsInvalid()) throw new ArgumentException("Invalid ImagePixels");

            var bitmap = new System.Drawing.Bitmap(pixels.Width, pixels.Height,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size), 
                System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

            try
            {
                RtlMoveMemory(bitmapData.Scan0, pixels.PixelsPtr, pixels.AllocSize);
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

            var bitmapSource = System.Windows.Media.Imaging.BitmapSource.Create(
                pixels.Width, pixels.Height, _dpi, _dpi,
                System.Windows.Media.PixelFormats.Bgr24, null,
                pixels.PixelsPtr, pixels.Height * pixels.Stride, pixels.Stride);

            bitmapSource.Freeze();
            return bitmapSource;
        }

        /// <summary>System.Windows.Media.Imaging.WriteableBitmap に変換します</summary>
        public static System.Windows.Media.Imaging.WriteableBitmap ToWriteableBitmap(this in ImagePixels pixels)
        {
            if (pixels.IsInvalid()) throw new ArgumentException("Invalid ImagePixels");

            var bitmap = new System.Windows.Media.Imaging.WriteableBitmap(
                pixels.Width, pixels.Height,
                _dpi, _dpi, System.Windows.Media.PixelFormats.Bgr24, null);

            bitmap.WritePixels(
                new System.Windows.Int32Rect(0, 0, pixels.Width, pixels.Height),
                pixels.PixelsPtr, pixels.AllocSize, pixels.Stride);

            return bitmap;
        }

        #endregion

    }
}
