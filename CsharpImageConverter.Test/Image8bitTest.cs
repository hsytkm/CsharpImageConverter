using CsharpImageConverter.Core;
using System;
using System.Linq;
using Xunit;

namespace CsharpImageConverter.Test
{
    public class Image8bitTest : ImageTestBase, IDisposable
    {
        public Image8bitTest() { }

        public void Dispose() { }

        [Fact]
        public void DrawingRead8bit()
        {
            using var bitmap1 = DrawingBitmapExtension.FromFile(@"Assets\image_gray.bmp")!;
            using var container2 = bitmap1.ToImagePixelsContainer();

            // DrawingBitmap -> ImagePixels
            var ave1 = bitmap1.GetChannelsAverage().ToArray();
            var ave2 = container2.Pixels.GetChannelsAverage().ToArray();
            ave1.Is(ave2);

            // ImagePixels -> DrawingBitmap
            using var bitmap3 = container2.Pixels.ToDrawingBitmap();
            var ave3 = bitmap3.GetChannelsAverage().ToArray();
            ave2.Is(ave3);

            // DrawingBitmap -> File -> DrawingBitmap
            const string savePath = @"_tempx.bmp";
            bitmap3.ToBmpFile(savePath);
            using var bitmap4 = DrawingBitmapExtension.FromFile(savePath)!;
            System.IO.File.Delete(savePath);
            var ave4 = bitmap4.GetChannelsAverage().ToArray();
            ave3.Is(ave4);
        }

        [Fact]
        public void BitmapSourceRead8bit()
        {
            var image = BitmapSourceExtension.FromFile(@"Assets\image_gray.bmp")!;
            using var container = image.ToImagePixelsContainer();

            var ave1 = image.GetChannelsAverage().ToArray();
            var ave2 = container.Pixels.GetChannelsAverage().ToArray();
            ave1.Is(ave2);
        }

    }
}
