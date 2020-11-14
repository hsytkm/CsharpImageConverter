using CsharpImageConverter.App.Common;
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
                new object[] { AppCommonSettings.BitmapFilePath, ".bmp" },
                new object[] { AppCommonSettings.PngFilePath, ".png" },
                new object[] { AppCommonSettings.TiffFilePath, ".tif" },
                new object[] { AppCommonSettings.JpegFilePath, ".jpg" },
            };

        /// <summary>無圧縮形式の画像ファイルPATHを取得(Load/Saveの比較テスト用)</summary>
        public static IEnumerable<object[]> GetUncompressedImageFilePaths =>
            new List<object[]>
            {
                new object[] { AppCommonSettings.BitmapFilePath, ".bmp" },
                new object[] { AppCommonSettings.PngFilePath, ".png" },
                new object[] { AppCommonSettings.TiffFilePath, ".tif" },
                //new object[] { AppCommonSettings.JpegFilePath, ".jpg" },
            };
        
    }
}
