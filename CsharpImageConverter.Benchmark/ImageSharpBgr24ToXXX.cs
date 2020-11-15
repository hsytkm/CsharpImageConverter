using BenchmarkDotNet.Attributes;
using CsharpImageConverter.Core;
using System;

namespace CsharpImageConverter.Benchmark
{
#if false

|                                 Method |       Mean |     Error |  StdDev |
|--------------------------------------- |-----------:|----------:|--------:|
|             ImageSharp_ToDrawingBitmap |   446.9 us |  20.82 us | 1.14 us |
|              ImageSharp_ToBitmapSource | 1,018.6 us | 136.37 us | 7.47 us |
|           ImageSharp_ToWriteableBitmap | 1,042.0 us | 154.58 us | 8.47 us |
| ImageSharp_ToWriteableBitmapWithFreeze | 1,179.9 us |  70.10 us | 3.84 us |
|       ImageSharp_CopyToWriteableBitmap |   272.7 us |   0.51 us | 0.03 us |
|           ImageSharp_CopyToImagePixels |   252.2 us |   7.15 us | 0.39 us |
#endif

    //[DryJob]        // 動作確認用の実行
    [ShortRunJob]   // 簡易測定
    public class ImageSharpBgr24ToXXX
    {
        private static readonly string _sourceBitmapPath = CoreContextSettings.BitmapFilePath;

        private System.Windows.Media.Imaging.WriteableBitmap _writeableBitmap = default!;
        private ImagePixelsContainer _container = default!;
        private SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Bgr24> _imageSharp = default!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _imageSharp = ImageSharpBgr24Extension.FromFile(_sourceBitmapPath);
            _writeableBitmap = _imageSharp.ToWriteableBitmap();
            _container = new ImagePixelsContainer(_imageSharp.Width, _imageSharp.Height);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _imageSharp?.Dispose();
            _container.Dispose();
        }

        [Benchmark]
        public void ImageSharp_ToDrawingBitmap()
        {
            var bitmap = _imageSharp.ToDrawingBitmap();
        }

        [Benchmark]
        public void ImageSharp_ToBitmapSource()
        {
            var bitmap = _imageSharp.ToBitmapSource();
        }

        [Benchmark]
        public void ImageSharp_ToWriteableBitmap()
        {
            var bitmap = _imageSharp.ToWriteableBitmap();
        }

        [Benchmark]
        public void ImageSharp_ToWriteableBitmapWithFreeze()
        {
            var bitmap = _imageSharp.ToWriteableBitmap();
            bitmap.Freeze();
        }

        [Benchmark]
        public void ImageSharp_CopyToWriteableBitmap()
        {
            _imageSharp.CopyToWriteableBitmap(_writeableBitmap);
        }

        [Benchmark]
        public void ImageSharp_CopyToImagePixels()
        {
            var pixels = _container.Pixels;
            _imageSharp.CopyToImagePixels(ref pixels);
        }


    }
}