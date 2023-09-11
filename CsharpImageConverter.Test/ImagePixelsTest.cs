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
            // bmp ���� ImagePixels ���쐬�ł��Ȃ��̂� DrawingBitmap ����쐬����
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

        [Theory]
        [MemberData(nameof(GetUncompressedImageFilePaths))]
        public void FileLoadSave(string imagePath, string extension)
        {
            // �摜�t�@�C���� Load -> Save -> Load ���ĉ�f���ϒl���r����
            using var bitmap0 = DrawingBitmapExtension.FromFile(imagePath);
            var baseAves = bitmap0.GetChannelsAverage().ToList();

            var savePath = GetTempFileName() + extension;

            try
            {
                using var container = bitmap0.ToImagePixelsContainer();
                container.Pixels.ToBmpFile(savePath);

                using var bitmap1 = DrawingBitmapExtension.FromFile(savePath);
                var newAves = bitmap1.GetChannelsAverage().ToList();
                newAves.Take(3).Is(baseAves.Take(3));   // 3ch�ɑ�����
            }
            finally
            {
                System.IO.File.Delete(savePath);    // Dispose��ɏ���
            }
        }
    }
}
