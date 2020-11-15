using CsharpImageConverter.Core;
using System;
using System.Linq;
using Xunit;

namespace CsharpImageConverter.Test
{
    public class BitmapSourceTest : ImageTestBase, IDisposable
    {
        private static readonly string _sourceBitmapPath = CoreContextSettings.BitmapFilePath;

        public BitmapSourceTest() { }

        public void Dispose() { }

        [Fact]
        public void ToEachClasses()
        {
            // bmp から作成した BitmapSource を各クラスに変換して、画素平均値を比較する
            var bitmapSource = BitmapSourceExtension.FromFile(_sourceBitmapPath);
            var avesBase = bitmapSource.GetChannelsAverage().ToList();
            avesBase.Count.Is(4);

            // ToDrawingBitmap
            using var bitmap0 = bitmapSource.ToDrawingBitmap();
            var aves00 = bitmap0.GetChannelsAverage().ToList();
            aves00.Count.Is(4);
            aves00.Is(avesBase);

            // ToWriteableBitmap(BitmapSource)
            var writeable = bitmapSource.ToWriteableBitmap();
            var aves10 = writeable.GetChannelsAverage().ToList();
            aves10.Count.Is(4);
            aves10.Is(avesBase);

            bitmapSource.CopyToWriteableBitmap(writeable);
            var aves11 = writeable.GetChannelsAverage().ToList();
            aves11.Count.Is(4);
            aves11.Is(avesBase);

            // ToImagePixelsContainer
            using var container = bitmapSource.ToImagePixelsContainer();
            var pixels = container.Pixels;
            var aves20 = pixels.GetChannelsAverage().ToList();
            aves20.Count.Is(3);
            aves20.Is(avesBase.Take(3));     // 3chに揃える

            bitmapSource.CopyToImagePixels(ref pixels);
            var aves21 = pixels.GetChannelsAverage().ToList();
            aves21.Count.Is(3);
            aves21.Is(avesBase.Take(3));     // 3chに揃える

            // ToImageSharpBgr24
            using var image = bitmapSource.ToImageSharpBgr24();
            var aves30 = image.GetChannelsAverage().ToList();
            aves30.Count.Is(3);
            aves30.Is(avesBase.Take(3));     // 3chに揃える

        }

        [Theory]
        [MemberData(nameof(GetUncompressedImageFilePaths))]
        public void FileLoadSave(string imagePath, string extension)
        {
            // 画像ファイルを Load -> Save -> Load して画素平均値を比較する
            var bitmap0 = BitmapSourceExtension.FromFile(imagePath);
            var baseAves = bitmap0.GetChannelsAverage().ToList();

            var savePath = GetTempFileName() + extension;
            bitmap0.ToImageFile(savePath);

            var bitmap1 = BitmapSourceExtension.FromFile(savePath);
            var newAves = bitmap1.GetChannelsAverage().ToList();
            newAves.Is(baseAves);

            System.IO.File.Delete(savePath);
        }

    }
}
