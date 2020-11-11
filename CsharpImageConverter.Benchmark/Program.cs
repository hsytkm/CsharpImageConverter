using BenchmarkDotNet.Running;
using System;

namespace CsharpImageConverter.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<MyTest>();
        }
    }
}
