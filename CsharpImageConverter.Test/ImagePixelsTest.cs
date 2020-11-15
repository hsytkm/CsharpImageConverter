using CsharpImageConverter.Core;
using System;
using System.Linq;
using Xunit;

namespace CsharpImageConverter.Test
{
    public class ImagePixelsTest : ImageTestBase, IDisposable
    {
        private static readonly string _sourceBitmapPath = CoreContextSettings.BitmapFilePath;

        public ImagePixelsTest() { }

        public void Dispose() { }

        [Fact]
        public void ToEachClasses()
        {
            // bmp Ç©ÇÁ ImagePixels ÇçÏê¨Ç≈Ç´Ç»Ç¢ÇÃÇ≈ DrawingBitmap Ç©ÇÁçÏê¨Ç∑ÇÈ
            using var bitmapBase = DrawingBitmapExtension.FromFile(_sourceBitmapPath);
            using var container = bitmapBase.ToImagePixelsContainer();
            var pixels = container.Pixels;
            var avesBase = pixels.GetChannelsAverage().ToList();
            avesBase.Count.Is(3);

            // ToDrawingBitmap
            using var bitmap0 = pixels.ToDrawingBitmap();
            var aves00 = bitmap0.GetChannelsAverage().ToList();
            aves00.Count.Is(3);
            aves00.Is(avesBase);

            // ToBitmapSource
            var bitmap1 = pixels.ToBitmapSource();
            var aves10 = bitmap1.GetChannelsAverage().ToList();
            aves10.Count.Is(3);
            aves10.Is(avesBase);

            // ToWriteableBitmap(BitmapSource)
            var writeable = pixels.ToWriteableBitmap();
            var aves20 = writeable.GetChannelsAverage().ToList();
            aves20.Count.Is(3);
            aves20.Is(avesBase);

            pixels.CopyToWriteableBitmap(writeable);
            var aves21 = writeable.GetChannelsAverage().ToList();
            aves21.Count.Is(3);
            aves21.Is(avesBase);

            // ToImageSharpBgr24
            using var image = pixels.ToImageSharpBgr24();
            var aves30 = image.GetChannelsAverage().ToList();
            aves30.Count.Is(3);
            aves30.Is(avesBase);

        }
    }
}
