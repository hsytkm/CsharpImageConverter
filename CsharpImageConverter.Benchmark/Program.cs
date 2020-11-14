using BenchmarkDotNet.Running;
using System;

namespace CsharpImageConverter.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
#if false
            // All tests
            BenchmarkRunner.Run<LoadImageFromFile>();
            BenchmarkRunner.Run<SaveImageToFile>();   //動作怪しい…
            BenchmarkRunner.Run<ReadPixels>();
            BenchmarkRunner.Run<DrawingBitmapToXXX>();
            BenchmarkRunner.Run<BitmapSourceToXXX>();
            BenchmarkRunner.Run<ImagePixelsToXXX>();
            BenchmarkRunner.Run<ImageSharpBgr24ToXXX>();
#else
            BenchmarkRunner.Run<LoadImageFromFile>();
#endif

        }
    }
}
