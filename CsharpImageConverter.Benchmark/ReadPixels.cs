using BenchmarkDotNet.Attributes;
using CsharpImageConverter.Core;
using System;
using System.Linq;

namespace CsharpImageConverter.Benchmark
{
#if false

|                   Method |     Mean |    Error |  StdDev |
|------------------------- |---------:|---------:|--------:|
| DrawingBitmap_GetAverage | 540.8 us | 27.69 us | 1.52 us |
|  BitmapSource_GetAverage | 899.5 us | 13.30 us | 0.73 us |
|   ImagePixels_GetAverage | 515.4 us | 21.13 us | 1.16 us |
|    ImageSharp_GetAverage | 518.1 us | 11.87 us | 0.65 us |
#endif

    //[DryJob]        // 動作確認用の実行
    [ShortRunJob]   // 簡易測定
    public class ReadPixels
    {
        private static readonly string _sourceBitmapPath = AppCommonSettings.BitmapFilePath;

        private System.Drawing.Bitmap _drawingBitmap = default!;
        private System.Windows.Media.Imaging.BitmapSource _bitmapSource = default!;
        private ImagePixelsContainer _container = default!;
        private SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Bgr24> _imageSharp = default!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _drawingBitmap = DrawingBitmapExtension.FromFile(_sourceBitmapPath);
            _bitmapSource = BitmapSourceExtension.FromFile(_sourceBitmapPath);
            _container = _drawingBitmap.ToImagePixelsContainer();
            _imageSharp = ImageSharpBgr24Extension.FromFile(_sourceBitmapPath);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _drawingBitmap?.Dispose();
            _container.Dispose();
            _imageSharp?.Dispose();
        }

        [Benchmark]
        public void DrawingBitmap_GetAverage()
        {
            var aves = _drawingBitmap.GetChannelsAverage();
        }

        [Benchmark]
        public void BitmapSource_GetAverage()
        {
            var aves = _bitmapSource.GetChannelsAverage();
        }
        
        [Benchmark]
        public void ImagePixels_GetAverage()
        {
            var pixels = _container.Pixels;
            var aves = pixels.GetChannelsAverage();
        }

        [Benchmark]
        public void ImageSharp_GetAverage()
        {
            var aves = _imageSharp.GetChannelsAverage();
        }

    }
}