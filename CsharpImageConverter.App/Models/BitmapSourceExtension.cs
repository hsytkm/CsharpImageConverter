using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace CsharpImageConverter.App.Models
{
    static class BitmapSourceExtension
    {
        #region FromFile
        /// <summary>引数PATHを画像として読み出す</summary>
        /// <param name="imagePath">ファイルパス</param>
        /// <returns></returns>
        public static BitmapImage FromFile(string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException(imagePath);

            static BitmapImage ToBitmapImage(Stream stream)
            {
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = stream;
                bi.EndInit();
                bi.Freeze();

                if (bi.Width == 1 && bi.Height == 1) throw new OutOfMemoryException();
                return bi;
            }

            using var stream = File.Open(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return ToBitmapImage(stream);

            //return new BitmapImage(new Uri(imagePath));  これでも読めるがファイルがロックされる
        }
        #endregion

        #region ToFile
        /// <summary>画像をファイルに保存する</summary>
        private static void ToFileBody(BitmapSource bitmapSource, string savePath, BitmapEncoder encoder)
        {
            if (File.Exists(savePath)) throw new ArgumentException("Save file is already exist.");

            using var fs = new FileStream(savePath, FileMode.Create);
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(fs);
        }

        /// <summary>画像をpngファイルに保存する</summary>
        public static void ToPngFile(this BitmapSource bitmapSource, string savePath)
            => ToFileBody(bitmapSource, savePath, new PngBitmapEncoder());

        /// <summary>画像をbmpファイルに保存する</summary>
        public static void ToBmpFile(this BitmapSource bitmapSource, string savePath)
            => ToFileBody(bitmapSource, savePath, new BmpBitmapEncoder());

        /// <summary>画像をjpgファイルに保存する</summary>
        public static void ToJpegFile(this BitmapSource bitmapSource, string savePath)
            => ToFileBody(bitmapSource, savePath, new JpegBitmapEncoder());

        #endregion
    }
}
