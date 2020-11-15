using System;

namespace CsharpImageConverter.Core
{
    public static class CoreContextSettings
    {
        internal const double DpiX = 96.0;
        internal const double DpiY = 96.0;

        public static string JpegFilePath => @"Assets\image.jpg";
        public static string BitmapFilePath => @"Assets\image.bmp";
        public static string PngFilePath => @"Assets\image.png";
        public static string TiffFilePath => @"Assets\image.tif";
    }
}
