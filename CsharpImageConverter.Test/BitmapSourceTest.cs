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
            // bmp ����쐬���� BitmapSource ���e�N���X�ɕϊ����āA��f���ϒl���r����
            var bitmapSource = BitmapSourceExtension.FromFile(_sourceBitmapPath);
            var avesBase = bitmapSource.GetChannelsAverage().ToList();
            avesBase.Count.Is(4);

            // ToDrawingBitmap
            using var bitmap0 = bitmapSource.ToDrawingBitmap();
            var aves00 = bitmap0.GetChannelsAverage().ToList();
            aves00.Count.Is(4);
            aves00.Is(avesBase);

            // ToWriteableBitmap(BitmapSource)
            var bitmap1 = bitmapSource.ToWriteableBitmap();
            var aves10 = bitmap0.GetChannelsAverage().ToList();
            aves10.Count.Is(4);
            aves10.Is(avesBase);

            // ToImagePixels
            using var container = bitmapSource.ToImagePixelsContainer();
            var aves20 = container.Pixels.GetChannelsAverage().ToList();
            aves20.Count.Is(3);
            aves20.Is(avesBase.Take(3));     // 3ch�ɑ�����

            // ToImageSharpBgr24
            using var image = bitmapSource.ToImageSharpBgr24();
            var aves30 = image.GetChannelsAverage().ToList();
            aves30.Count.Is(3);
            aves30.Is(avesBase.Take(3));     // 3ch�ɑ�����

        }

        [Theory]
        [MemberData(nameof(GetUncompressedImageFilePaths))]
        public void FileLoadSave(string imagePath, string extension)
        {
            // �摜�t�@�C���� Load -> Save -> Load ���ĉ�f���ϒl���r����
            var bitmap0 = BitmapSourceExtension.FromFile(imagePath);
            var baseAves = bitmap0.GetChannelsAverage().ToList();

            var savePath = System.IO.Path.GetTempFileName();
            System.IO.File.Delete(savePath);

            savePath += extension;
            bitmap0.ToImageFile(savePath);

            var bitmap1 = BitmapSourceExtension.FromFile(savePath);
            var newAves = bitmap1.GetChannelsAverage().ToList();
            newAves.Is(baseAves);

            System.IO.File.Delete(savePath);
        }

    }
}
