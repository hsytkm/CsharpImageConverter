using BenchmarkDotNet.Attributes;
using CsharpImageConverter.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CsharpImageConverter.Benchmark
{
    //[DryJob]        // 動作確認用の実行
    [ShortRunJob]   // 簡易測定
    public class MyTest
    {
        private readonly int[] _array;

        public MyTest()
        {
            _array = Enumerable.Range(0, 10000).ToArray();

            var pixels = new ImagePixels();
        }

        /// <summary>
        /// 早いけど最速ではない
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public int UseArrayFor1()
        {
            var accum = 0;

            for (int i = 0; i < _array.Length; i++)
            {
                accum += _array[i];
            }
            return accum;
        }

        /// <summary>
        /// ループ外でローカル変数にコピーした方がちょい早く最速
        /// Lengthのチェックが必要なくなったおかげと思われる。IL見てないから知らんけど
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public int UseArrayFor2()
        {
            var accum = 0;
            var array = _array;
            for (int i = 0; i < array.Length; i++)
            {
                accum += array[i];
            }
            return accum;
        }

    }
}