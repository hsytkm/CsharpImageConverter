using BenchmarkDotNet.Attributes;
using CsharpImageConverter.Core;
using System;

namespace CsharpImageConverter.Benchmark
{
#if false

|                                  Method |      Mean |      Error |    StdDev |
|---------------------------------------- |----------:|-----------:|----------:|
|              ImagePixels_ToBitmapSource | 160.86 us | 716.737 us | 39.287 us |
|           ImagePixels_ToWriteableBitmap | 874.61 us | 129.638 us |  7.106 us |
| ImagePixels_ToWriteableBitmapWithFreeze | 903.02 us |  84.138 us |  4.612 us |
|       ImagePixels_CopyToWriteableBitmap |  27.82 us |   6.651 us |  0.365 us |
|         DrawingBitmap_ToImageSharpBgr24 | 701.33 us | 192.634 us | 10.559 us |
#endif

    //[DryJob]        // 動作確認用の実行
    [ShortRunJob]   // 簡易測定
    public class ImagePixelsToXXX
    {
        private static readonly string _sourceBitmapPath = AppCommonSettings.BitmapFilePath;

        private System.Windows.Media.Imaging.WriteableBitmap _writeableBitmap = default!;
        private ImagePixelsContainer _container = default!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            using var drawingBitmap = DrawingBitmapExtension.FromFile(_sourceBitmapPath);
            _writeableBitmap = drawingBitmap.ToWriteableBitmap();
            _container = new ImagePixelsContainer(drawingBitmap.Width, drawingBitmap.Height);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _container.Dispose();
        }

        [Benchmark]
        public void ImagePixels_ToBitmapSource()
        {
            var bitmap = _container.Pixels.ToBitmapSource();
        }

        [Benchmark]
        public void ImagePixels_ToWriteableBitmap()
        {
            var bitmap = _container.Pixels.ToWriteableBitmap();
        }

        [Benchmark]
        public void ImagePixels_ToWriteableBitmapWithFreeze()
        {
            var bitmap = _container.Pixels.ToWriteableBitmap();
            bitmap.Freeze();
        }

        [Benchmark]
        public void ImagePixels_CopyToWriteableBitmap()
        {
            _container.Pixels.CopyToWriteableBitmap(_writeableBitmap);
        }

        [Benchmark]
        public void DrawingBitmap_ToImageSharpBgr24()
        {
            var image = _container.Pixels.ToImageSharpBgr24();
        }

    }
}