using BenchmarkDotNet.Attributes;
using CsharpImageConverter.Core;
using System;

namespace CsharpImageConverter.Benchmark
{
#if false
|                                    Method |        Mean |     Error |    StdDev |
|------------------------------------------ |------------:|----------:|----------:|
|             DrawingBitmap_ToBitmapSource1 | 4,351.07 us | 406.81 us | 22.299 us |
|             DrawingBitmap_ToBitmapSource2 | 4,317.91 us | 492.31 us | 26.985 us |
|          DrawingBitmap_ToBitmapSourceFast | 1,739.42 us | 894.54 us | 49.033 us |
|           DrawingBitmap_ToWriteableBitmap |   863.40 us |  65.53 us |  3.592 us |
| DrawingBitmap_ToWriteableBitmapWithFreeze |   925.11 us | 485.47 us | 26.610 us |
|       DrawingBitmap_CopyToWriteableBitmap |    27.91 us |  10.53 us |  0.577 us |
|           DrawingBitmap_CopyToImagePixels |    38.00 us |  10.27 us |  0.563 us |
|           DrawingBitmap_ToImageSharpBgr24 |   683.39 us |  50.68 us |  2.778 us |
#endif

    //[DryJob]        // 動作確認用の実行
    [ShortRunJob]   // 簡易測定
    public class DrawingBitmapToXXX
    {
        private static readonly string _sourceBitmapPath = CoreContextSettings.BitmapFilePath;

        private System.Drawing.Bitmap _drawingBitmap = default!;
        private System.Windows.Media.Imaging.WriteableBitmap _writeableBitmap = default!;
        private ImagePixelsContainer _container = default!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _drawingBitmap = DrawingBitmapExtension.FromFile(_sourceBitmapPath);
            _writeableBitmap = _drawingBitmap.ToWriteableBitmap();
            _container = new ImagePixelsContainer(_drawingBitmap.Width, _drawingBitmap.Height, _drawingBitmap.GetBytesPerPixel());
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _drawingBitmap?.Dispose();
            _container.Dispose();
        }

        [Benchmark]
        public void DrawingBitmap_ToBitmapSource1()
        {
            var bitmap = _drawingBitmap.ToBitmapSource1();
        }

        [Benchmark]
        public void DrawingBitmap_ToBitmapSource2()
        {
            var bitmap = _drawingBitmap.ToBitmapSource2();
        }

        [Benchmark]
        public void DrawingBitmap_ToBitmapSourceFast()
        {
            var bitmap = _drawingBitmap.ToBitmapSourceFast();
        }

        [Benchmark]
        public void DrawingBitmap_ToWriteableBitmap()
        {
            var bitmap = _drawingBitmap.ToWriteableBitmap();
        }

        [Benchmark]
        public void DrawingBitmap_ToWriteableBitmapWithFreeze()
        {
            var bitmap = _drawingBitmap.ToWriteableBitmap();
            bitmap.Freeze();
        }

        [Benchmark]
        public void DrawingBitmap_CopyToWriteableBitmap()
        {
            _drawingBitmap.CopyToWriteableBitmap(_writeableBitmap);
        }

        [Benchmark]
        public void DrawingBitmap_CopyToImagePixels()
        {
            var pixels = _container.Pixels;
            _drawingBitmap.CopyToImagePixels(ref pixels);
        }

        [Benchmark]
        public void DrawingBitmap_ToImageSharpBgr24()
        {
            var image = _drawingBitmap.ToImageSharpBgr24();
        }

    }
}