using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CsharpImageConverter.App.Models
{
    static class ImagePixelsExtension
    {
#if false
        public static Bitmap ToDrawingBitmap(this ImagePixels pixels)
        {

        }

        public static BitmapSource ToBitmapSource(this ImagePixels pixels)
        {
            if (!pixels.IsValid()) throw new ArgumentException();

            var bitmap = new WriteableBitmap(pixels.Width, pixels.Height,
                96.0, 96.0, PixelFormats.Bgr24, null);

            bitmap.WritePixels(new Int32Rect(0, 0, pixels.Width, pixels.Height),
                pixels.PixelsPtr, pixels.AllocSize, pixels.Stride);

            return bitmap;
        }

#endif
    }
}
