using System;
using System.Collections.Generic;
using System.Linq;

namespace CsharpImageConverter.App.Common
{
    /// <summary>画像ファイル形式</summary>
    public enum ImageFileFormat
    {
        PNG, JPEG, BMP,
    }

    /// <summary>画像クラス</summary>
    public enum ImageClass
    {
        DrawingBitmap, BitmapSource, MyImagePixels
    }

    static class ImageEnumExtension
    {
        /// <summary>引数画像形式を読込みできるクラス</summary>
        public static IEnumerable<ImageClass> CanReadClasses(this ImageFileFormat sourceFormat)
        {
            return new[]
            {
                ImageClass.DrawingBitmap
            };
        }

    }


    static class EnumExtension
    {
        public static IEnumerable<T> GetItems<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

    }
}
