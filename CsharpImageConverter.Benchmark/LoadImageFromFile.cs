using BenchmarkDotNet.Attributes;
using CsharpImageConverter.Core;
using System;
using System.Threading.Tasks;

namespace CsharpImageConverter.Benchmark
{
#if false
|                     Method |        Mean |       Error |    StdDev |
|--------------------------- |------------:|------------:|----------:|
|     DrawingBitmap_FromJpeg |  2,778.0 us |   109.09 us |   5.98 us |
|   DrawingBitmap_FromBitmap |    965.7 us |    70.37 us |   3.86 us |
|      DrawingBitmap_FromPng |  5,122.6 us | 1,471.77 us |  80.67 us |
|     DrawingBitmap_FromTiff |  8,287.3 us |   464.31 us |  25.45 us |
|      BitmapSource_FromJpeg | 10,336.5 us |   225.89 us |  12.38 us |
|    BitmapSource_FromBitmap |  4,157.3 us |   306.82 us |  16.82 us |
|       BitmapSource_FromPng |  5,542.6 us |   466.31 us |  25.56 us |
|      BitmapSource_FromTiff | 10,200.8 us |   178.14 us |   9.76 us |
|        ImageSharp_FromJpeg |  4,295.2 us |   246.36 us |  13.50 us |
|      ImageSharp_FromBitmap |  1,159.5 us |    35.78 us |   1.96 us |
|         ImageSharp_FromPng |  7,851.6 us |    78.10 us |   4.28 us |
|   ImageSharp_FromJpegAsync |  4,294.1 us |    48.01 us |   2.63 us |
| ImageSharp_FromBitmapAsync |  1,166.6 us |    76.15 us |   4.17 us |
|    ImageSharp_FromPngAsync |  7,905.1 us | 1,857.70 us | 101.83 us |
#endif

    //[DryJob]        // 動作確認用の実行
    [ShortRunJob]   // 簡易測定
    public class LoadImageFromFile
    {
        private static readonly string _jpgPath = CoreContextSettings.JpegFilePath;
        private static readonly string _bmpPath = CoreContextSettings.BitmapFilePath;
        private static readonly string _pngPath = CoreContextSettings.PngFilePath;
        private static readonly string _tifPath = CoreContextSettings.TiffFilePath;

        public LoadImageFromFile()
        {
        }

        #region DrawingBitmap
        [Benchmark]
        public void DrawingBitmap_FromJpeg()
        {
            var bitmap = DrawingBitmapExtension.FromFile(_jpgPath);
        }

        [Benchmark]
        public void DrawingBitmap_FromBitmap()
        {
            var bitmap = DrawingBitmapExtension.FromFile(_bmpPath);
        }

        [Benchmark]
        public void DrawingBitmap_FromPng()
        {
            var bitmap = DrawingBitmapExtension.FromFile(_pngPath);
        }

        [Benchmark]
        public void DrawingBitmap_FromTiff()
        {
            var bitmap = DrawingBitmapExtension.FromFile(_tifPath);
        }
        #endregion

        #region BitmapSource
        [Benchmark]
        public void BitmapSource_FromJpeg()
        {
            var bitmap = BitmapSourceExtension.FromFile(_jpgPath);
        }

        [Benchmark]
        public void BitmapSource_FromBitmap()
        {
            var bitmap = BitmapSourceExtension.FromFile(_bmpPath);
        }

        [Benchmark]
        public void BitmapSource_FromPng()
        {
            var bitmap = BitmapSourceExtension.FromFile(_pngPath);
        }

        [Benchmark]
        public void BitmapSource_FromTiff()
        {
            var bitmap = BitmapSourceExtension.FromFile(_tifPath);
        }
        #endregion

        #region ImageSharp
        [Benchmark]
        public void ImageSharp_FromJpeg()
        {
            var bitmap = ImageSharpBgr24Extension.FromFile(_jpgPath);
        }

        [Benchmark]
        public void ImageSharp_FromBitmap()
        {
            var bitmap = ImageSharpBgr24Extension.FromFile(_bmpPath);
        }

        [Benchmark]
        public void ImageSharp_FromPng()
        {
            var bitmap = ImageSharpBgr24Extension.FromFile(_pngPath);
        }

        [Benchmark]
        public async Task ImageSharp_FromJpegAsync()
        {
            var bitmap = await ImageSharpBgr24Extension.FromFileAsync(_jpgPath);
        }

        [Benchmark]
        public async Task ImageSharp_FromBitmapAsync()
        {
            var bitmap = await ImageSharpBgr24Extension.FromFileAsync(_bmpPath);
        }

        [Benchmark]
        public async Task ImageSharp_FromPngAsync()
        {
            var bitmap = await ImageSharpBgr24Extension.FromFileAsync(_pngPath);
        }
        #endregion

    }
}