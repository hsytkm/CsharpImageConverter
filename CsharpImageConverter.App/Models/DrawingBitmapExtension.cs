using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CsharpImageConverter.App.Models
{
    static class DrawingBitmapExtension
    {
        #region FromFile
        /// <summary>引数PATHを画像として読み出す</summary>
        /// <param name="imagePath">ファイルパス</param>
        /// <returns></returns>
        public static Bitmap FromFile(string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException(imagePath);
            return new Bitmap(imagePath);
        }
        #endregion

        #region ToFile
        /// <summary>引数PATHを画像として読み出す</summary>
        /// <param name="imagePath">ファイルパス</param>
        /// <returns></returns>
        public static void ToFileBody(Bitmap bitmap, string savePath, ImageFormat format)
        {
            if (File.Exists(savePath)) throw new ArgumentException("Save file is already exist.");
            bitmap.Save(savePath, format);
        }

        /// <summary>画像をpngファイルに保存する</summary>
        public static void ToPngFile(this Bitmap bitmap, string savePath)
            => ToFileBody(bitmap, savePath, ImageFormat.Png);

        /// <summary>画像をbmpファイルに保存する</summary>
        public static void ToBmpFile(this Bitmap bitmap, string savePath)
            => ToFileBody(bitmap, savePath, ImageFormat.Bmp);

        /// <summary>画像をjpgファイルに保存する</summary>
        public static void ToJpegFile(this Bitmap bitmap, string savePath)
            => ToFileBody(bitmap, savePath, ImageFormat.Jpeg);

        #endregion

    }
}
