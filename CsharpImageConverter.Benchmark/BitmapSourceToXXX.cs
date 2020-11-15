using BenchmarkDotNet.Attributes;
using CsharpImageConverter.Core;
using System;

namespace CsharpImageConverter.Benchmark
{
#if false

|                                    Method |       Mean |       Error |   StdDev |
|------------------------------------------ |-----------:|------------:|---------:|
|              BitmapSource_ToDrawingBitmap |   364.7 us |   270.61 us | 14.83 us |
|           DrawingBitmap_ToWriteableBitmap | 1,532.6 us |    63.48 us |  3.48 us |
| DrawingBitmap_ToWriteableBitmapWithFreeze | 1,605.4 us |   376.55 us | 20.64 us |
|       DrawingBitmap_CopyToWriteableBitmap |   628.1 us |   112.04 us |  6.14 us |
|                BitmapSource_ToImagePixels |   579.5 us |    40.89 us |  2.24 us |
|           DrawingBitmap_ToImageSharpBgr24 | 1,133.7 us | 1,244.47 us | 68.21 us |
#endif

    //[DryJob]        // 動作確認用の実行
    [ShortRunJob]   // 簡易測定
    public class BitmapSourceToXXX
    {
        private static readonly string _sourceBitmapPath = CoreContextSettings.BitmapFilePath;

        private System.Windows.Media.Imaging.BitmapSource _bitmapSource = default!;
        private System.Windows.Media.Imaging.WriteableBitmap _writeableBitmap = default!;
        private ImagePixelsContainer _container = default!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _bitmapSource = BitmapSourceExtension.FromFile(_sourceBitmapPath);
            _writeableBitmap = _bitmapSource.ToWriteableBitmap();
            _container = new ImagePixelsContainer(_bitmapSource.PixelWidth, _bitmapSource.PixelHeight);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _container.Dispose();
        }

        [Benchmark]
        public void BitmapSource_ToDrawingBitmap()
        {
            var bitmap = _bitmapSource.ToDrawingBitmap();
        }

        [Benchmark]
        public void DrawingBitmap_ToWriteableBitmap()
        {
            var bitmap = _bitmapSource.ToWriteableBitmap();
        }

        [Benchmark]
        public void DrawingBitmap_ToWriteableBitmapWithFreeze()
        {
            var bitmap = _bitmapSource.ToWriteableBitmap();
            bitmap.Freeze();
        }

        [Benchmark]
        public void DrawingBitmap_CopyToWriteableBitmap()
        {
            _bitmapSource.CopyToWriteableBitmap(_writeableBitmap);
        }

        [Benchmark]
        public void BitmapSource_ToImagePixels()
        {
            var pixels = _container.Pixels;
            _bitmapSource.CopyToImagePixels(ref pixels);
        }

        [Benchmark]
        public void DrawingBitmap_ToImageSharpBgr24()
        {
            var image = _bitmapSource.ToImageSharpBgr24();
        }

    }
}