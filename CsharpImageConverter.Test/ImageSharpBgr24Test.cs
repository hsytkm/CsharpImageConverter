using CsharpImageConverter.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CsharpImageConverter.Test
{
    public class ImageSharpBgr24Test : ImageTestBase, IDisposable
    {
        private static readonly string _sourceBitmapPath = CoreContextSettings.BitmapFilePath;

        public ImageSharpBgr24Test() { }

        public void Dispose() { }

        [Fact]
        public async Task ToEachClasses()
        {
            // bmp から ImagePixels を作成できないので DrawingBitmap から作成する
            using var image = await ImageSharpBgr24Extension.FromFileAsync(_sourceBitmapPath);
            var avesBase = image.GetChannelsAverage().ToList();
            avesBase.Count.Is(3);

            // ToDrawingBitmap
            using var bitmap0 = image.ToDrawingBitmap();
            var aves00 = bitmap0.GetChannelsAverage().ToList();
            aves00.Count.Is(3);
            aves00.Is(avesBase);

            // ToBitmapSource
            var bitmapSource = image.ToBitmapSource();
            var aves10 = bitmapSource.GetChannelsAverage().ToList();
            aves10.Count.Is(3);
            aves10.Is(avesBase);

            // ToWriteableBitmap(BitmapSource)
            var writeable = image.ToWriteableBitmap();
            var aves20 = writeable.GetChannelsAverage().ToList();
            aves20.Count.Is(3);
            aves20.Is(avesBase);

            image.CopyToWriteableBitmap(writeable);
            var aves21 = writeable.GetChannelsAverage().ToList();
            aves21.Count.Is(3);
            aves21.Is(avesBase);

            // ToImagePixelsContainer
            using var container = image.ToImagePixelsContainer();
            var pixels = container.Pixels;
            var aves30 = pixels.GetChannelsAverage().ToList();
            aves30.Count.Is(3);
            aves30.Is(avesBase);

            image.CopyToImagePixels(ref pixels);
            var aves31 = pixels.GetChannelsAverage().ToList();
            aves31.Count.Is(3);
            aves31.Is(avesBase);
        }

        [Theory]
        [MemberData(nameof(GetUncompressedImageFilePaths))]
        public void FileLoadSave(string imagePath, string extension)
        {
            if (extension.Contains(".tif")) return;    //ImageSharp(1.02) は Tiff に未対応…

            // 画像ファイルを Load -> Save -> Load して画素平均値を比較する
            using var bitmap0 = ImageSharpBgr24Extension.FromFile(imagePath);
            var baseAves = bitmap0.GetChannelsAverage().ToList();

            var savePath = GetTempFileName() + extension;
            bitmap0.ToImageFile(savePath);

            using var bitmap1 = ImageSharpBgr24Extension.FromFile(savePath);
            var newAves = bitmap1.GetChannelsAverage().ToList();
            newAves.Is(baseAves);

            System.IO.File.Delete(savePath);
        }

        [Theory]
        [MemberData(nameof(GetUncompressedImageFilePaths))]
        public async Task FileLoadSaveAsync(string imagePath, string extension)
        {
            if (extension.Contains(".tif")) return;    //ImageSharp(1.02) は Tiff に未対応…

            // 画像ファイルを Load -> Save -> Load して画素平均値を比較する
            using var bitmap0 = await ImageSharpBgr24Extension.FromFileAsync(imagePath);
            var baseAves = bitmap0.GetChannelsAverage().ToList();

            var savePath = GetTempFileName() + extension;
            await bitmap0.ToImageFileAsync(savePath);

            using var bitmap1 = await ImageSharpBgr24Extension.FromFileAsync(savePath);
            var newAves = bitmap1.GetChannelsAverage().ToList();
            newAves.Is(baseAves);

            System.IO.File.Delete(savePath);
        }

    }
}
