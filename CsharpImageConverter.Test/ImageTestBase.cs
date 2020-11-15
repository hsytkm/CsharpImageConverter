using CsharpImageConverter.Core;
using System;
using System.Collections.Generic;

namespace CsharpImageConverter.Test
{
    public abstract class ImageTestBase
    {
        /// <summary>各画像ファイルPATHを取得</summary>
        public static IEnumerable<object[]> GetAllImageFilePaths =>
            new List<object[]>
            {
                new object[] { CoreContextSettings.BitmapFilePath, ".bmp" },
                new object[] { CoreContextSettings.PngFilePath, ".png" },
                new object[] { CoreContextSettings.TiffFilePath, ".tif" },
                new object[] { CoreContextSettings.JpegFilePath, ".jpg" },
            };

        /// <summary>無圧縮形式の画像ファイルPATHを取得(Load/Saveの比較テスト用)</summary>
        public static IEnumerable<object[]> GetUncompressedImageFilePaths =>
            new List<object[]>
            {
                new object[] { CoreContextSettings.BitmapFilePath, ".bmp" },
                new object[] { CoreContextSettings.PngFilePath, ".png" },
                new object[] { CoreContextSettings.TiffFilePath, ".tif" },
                //new object[] { AppCommonSettings.JpegFilePath, ".jpg" },
            };

        /// <summary>一時ファイルPATHを返します</summary>
        internal static string GetTempFileName()
        {
            var path = System.IO.Path.GetTempFileName();
            System.IO.File.Delete(path);
            return path;
        }
    }
}
