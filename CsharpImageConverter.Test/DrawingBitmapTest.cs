using CsharpImageConverter.Core;
using System;
using System.Linq;
using Xunit;

namespace CsharpImageConverter.Test
{
    public class DrawingBitmapTest : ImageTestBase, IDisposable
    {
        private static readonly string _sourceBitmapPath = CoreContextSettings.BitmapFilePath;

        public DrawingBitmapTest() { }

        public void Dispose() { }

        [Fact]
        public void ToEachClasses()
        {
            // bmp から作成した DrawingBitmap を各クラスに変換して、画素平均値を比較する
            using var bitmap = DrawingBitmapExtension.FromFile(_sourceBitmapPath);
            var avesBase = bitmap.GetChannelsAverage().ToList();
            avesBase.Count.Is(3);

            // ToBitmapSource
            var bitmapSource0 = bitmap.ToBitmapSource1();
            var aves00 = bitmapSource0.GetChannelsAverage().ToList();
            aves00.Count.Is(4);
            aves00.Take(3).Is(avesBase);  // bitmapSourceは 4ch なので 3ch に揃えて比較

            var bitmapSource1 = bitmap.ToBitmapSource2();
            var aves01 = bitmapSource1.GetChannelsAverage().ToList();
            aves01.Count.Is(4);
            aves01.Take(3).Is(avesBase);  // bitmapSourceは 4ch なので 3ch に揃えて比較

            var bitmapSource2 = bitmap.ToBitmapSourceFast();
            var aves02 = bitmapSource2.GetChannelsAverage().ToList();
            aves02.Count.Is(4);
            aves02.Take(3).Is(avesBase);  // bitmapSourceは 4ch なので 3ch に揃えて比較

            // ToWriteableBitmap(BitmapSource)
            var writeable = bitmap.ToWriteableBitmap();
            var aves10 = writeable.GetChannelsAverage().ToList();
            aves10.Count.Is(3);
            aves10.Is(avesBase);

            // ToImagePixels
            using var container = bitmap.ToImagePixelsContainer();
            var aves20 = container.Pixels.GetChannelsAverage().ToList();
            aves20.Count.Is(3);
            aves20.Is(avesBase);

            // ToImageSharpBgr24
            using var image = bitmap.ToImageSharpBgr24();
            var aves30 = image.GetChannelsAverage().ToList();
            aves30.Count.Is(3);
            aves30.Is(avesBase);

        }

        [Theory]
        [MemberData(nameof(GetUncompressedImageFilePaths))]
        public void FileLoadSave(string imagePath, string extension)
        {
            // 画像ファイルを Load -> Save -> Load して画素平均値を比較する
            using var bitmap0 = DrawingBitmapExtension.FromFile(imagePath);
            var baseAves = bitmap0.GetChannelsAverage().ToList();

            var savePath = System.IO.Path.GetTempFileName();
            System.IO.File.Delete(savePath);

            try
            {
                savePath += extension;
                bitmap0.ToImageFile(savePath);

                using var bitmap1 = DrawingBitmapExtension.FromFile(savePath);
                var newAves = bitmap1.GetChannelsAverage().ToList();
                newAves.Is(baseAves);
            }
            finally
            {
                System.IO.File.Delete(savePath);    // Dispose後に消す
            }
        }

    }
}
