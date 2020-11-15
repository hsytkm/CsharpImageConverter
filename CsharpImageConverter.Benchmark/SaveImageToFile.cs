using BenchmarkDotNet.Attributes;
using CsharpImageConverter.Core;
using System;
using System.IO;

namespace CsharpImageConverter.Benchmark
{
#if false
|                 Method |      Mean |     Error |    StdDev |
|----------------------- |----------:|----------:|----------:|
|   DrawingBitmap_ToJpeg |  3.593 ms |  2.145 ms | 0.1176 ms |
| DrawingBitmap_ToBitmap |  4.828 ms |  5.272 ms | 0.2890 ms |
|    DrawingBitmap_ToPng | 20.908 ms |  8.086 ms | 0.4432 ms |
|   DrawingBitmap_ToTiff | 14.579 ms | 34.937 ms | 1.9150 ms |
|    BitmapSource_ToJpeg |  4.135 ms |  3.022 ms | 0.1656 ms |
|  BitmapSource_ToBitmap | 10.200 ms |  5.086 ms | 0.2788 ms |
|     BitmapSource_ToPng | 20.788 ms | 18.523 ms | 1.0153 ms |
|    BitmapSource_ToTiff | 14.529 ms |  5.807 ms | 0.3183 ms |
#endif

    //[DryJob]        // 動作確認用の実行
    [ShortRunJob]   // 簡易測定
    public class SaveImageToFile
    {
        private static readonly string _sourceBitmapPath = AppCommonSettings.BitmapFilePath;

        private string _saveFilePath = default!;
        private System.Drawing.Bitmap _drawingBitmap = default!;
        private System.Windows.Media.Imaging.BitmapSource _bitmapSource = default!;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _drawingBitmap = DrawingBitmapExtension.FromFile(_sourceBitmapPath);
            _bitmapSource = BitmapSourceExtension.FromFile(_sourceBitmapPath);
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _drawingBitmap?.Dispose();
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _saveFilePath = Path.GetTempFileName();
            File.Delete(_saveFilePath);
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            File.Delete(_saveFilePath);
        }

        [Benchmark]
        public void DrawingBitmap_ToJpeg()
        {
            _drawingBitmap.ToJpegFile(_saveFilePath);
        }

        [Benchmark]
        public void DrawingBitmap_ToBitmap()
        {
            _drawingBitmap.ToBmpFile(_saveFilePath);
        }

        [Benchmark]
        public void DrawingBitmap_ToPng()
        {
            _drawingBitmap.ToPngFile(_saveFilePath);
        }

        [Benchmark]
        public void DrawingBitmap_ToTiff()
        {
            _drawingBitmap.ToTiffFile(_saveFilePath);
        }

        [Benchmark]
        public void BitmapSource_ToJpeg()
        {
            _bitmapSource.ToJpegFile(_saveFilePath);
        }

        [Benchmark]
        public void BitmapSource_ToBitmap()
        {
            _bitmapSource.ToBmpFile(_saveFilePath);
        }

        [Benchmark]
        public void BitmapSource_ToPng()
        {
            _bitmapSource.ToPngFile(_saveFilePath);
        }

        [Benchmark]
        public void BitmapSource_ToTiff()
        {
            _bitmapSource.ToTiffFile(_saveFilePath);
        }
    }
}