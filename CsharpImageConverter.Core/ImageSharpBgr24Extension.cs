using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CsharpImageConverter.Core
{
    public static class ImageSharpBgr24Extension
    {
        /// <summary>Imageに異常がないかチェックします</summary>
        public static bool IsValid(this Image image)
        {
            if (image.Width == 0 || image.Height == 0) return false;
            return true;
        }

        /// <summary>Imageに異常がないかチェックします</summary>
        public static bool IsInvalid(this Image image) => !IsValid(image);

        /// <summary>1PixelのByte数を取得します</summary>
        public static int GetBytesPerPixel(this Image image)
        {
            if (image.IsInvalid()) throw new ArgumentException("Invalid Image");
            return Ceiling(image.PixelType.BitsPerPixel, 8);

            static int Ceiling(int value, int div) => (value + (div - 1)) / div;
        }

        #region FromFile
        /// <summary>引数PATHのファイルを画像として読み出します</summary>
        public static Image<Bgr24> FromFile(string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException(imagePath);

            return Image.Load<Bgr24>(imagePath);
        }

        /// <summary>引数PATHのファイルを画像として読み出します</summary>
        public static async Task<Image<Bgr24>> FromFileAsync(string imagePath)
        {
            if (!File.Exists(imagePath)) throw new FileNotFoundException(imagePath);

            return await Image.LoadAsync<Bgr24>(imagePath);
        }
        #endregion

        #region ToFile
        /// <summary>保存ファイルPATHの拡張子に応じた画像を保存します</summary>
        public static void ToImageFile(this Image image, string savePath)
        {
            if (File.Exists(savePath)) throw new ArgumentException("Save file is already exist.");

            image.Save(savePath);
        }

        /// <summary>保存ファイルPATHの拡張子に応じた画像を保存します</summary>
        public static async Task ToImageFileAsync(this Image image, string savePath)
        {
            if (File.Exists(savePath)) throw new ArgumentException("Save file is already exist.");

            await image.SaveAsync(savePath);
        }
        #endregion

        #region ReadPixels
        /// <summary>指定エリアの画素平均値を取得します</summary>
        public static IReadOnlyCollection<double> GetChannelsAverage(this Image<Bgr24> image, in Rectangle rect)
        {
            if (image.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (rect.Width * rect.Height == 0) throw new ArgumentException("Area is zero.");
            if (image.Width < rect.X + rect.Width) throw new ArgumentException("Width over.");
            if (image.Height < rect.Y + rect.Height) throw new ArgumentException("Height over.");

            var bytesPerPixel = image.GetBytesPerPixel();
            if (bytesPerPixel != 3) throw new NotSupportedException("BytesPerPixel");

            Span<ulong> sumChannels = stackalloc ulong[bytesPerPixel];

            var height = image.Height;
            var width = image.Width;
            for (var y = 0; y < height; ++y)
            {
                var pixelRowSpan = image.GetPixelRowSpan(y);
                for (var x = 0; x < width; ++x)
                {
                    ref var bgr = ref pixelRowSpan[x];
                    sumChannels[0] += bgr.B;
                    sumChannels[1] += bgr.G;
                    sumChannels[2] += bgr.R;
                }
            }

            var aveChannels = new double[sumChannels.Length];
            var count = (double)(rect.Width * rect.Height);

            for (var i = 0; i < aveChannels.Length; ++i)
            {
                aveChannels[i] = sumChannels[i] / count;
            }
            return aveChannels;
        }

        /// <summary>画面全体の画素平均値を取得します</summary>
        public static IEnumerable<double> GetChannelsAverage(this Image<Bgr24> image)
        {
            if (image.IsInvalid()) throw new ArgumentException("Invalid Image");

            var fullRect = new Rectangle(0, 0, image.Width, image.Height);
            return GetChannelsAverage(image, fullRect);
        }
        #endregion

        #region ToDrawingBitmap
        /// <summary>System.Drawing.Bitmap に変換します</summary>
        public static System.Drawing.Bitmap ToDrawingBitmap(this Image<Bgr24> image)
        {
            if (image.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (image.GetBytesPerPixel() != 3) throw new ArgumentException("Invalid BytesPerPixel");

            var bitmap = new System.Drawing.Bitmap(image.Width, image.Height,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

            try
            {
                CopyImagePixelsImpl(image, bitmapData.Scan0, bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
            return bitmap;
        }
        #endregion

        #region ToBitmapSource
        /// <summary>System.Windows.Media.Imaging.BitmapSource に変換します</summary>
        public static System.Windows.Media.Imaging.BitmapSource ToBitmapSource(this Image<Bgr24> image)
        {
            if (image.IsInvalid()) throw new ArgumentException("Invalid Image");

            // 一応用意したが ToWriteableBitmap() を直でコールすれば良いと思う
            return ToWriteableBitmap(image);
        }
        #endregion

        #region ToWriteableBitmap
        /// <summary>System.Windows.Media.Imaging.WriteableBitmap の画素値を更新します</summary>
        public static void CopyToWriteableBitmap(this Image<Bgr24> image, System.Windows.Media.Imaging.WriteableBitmap writeableBitmap)
        {
            if (image.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (writeableBitmap.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (writeableBitmap.PixelWidth != image.Width) throw new ArgumentException("Different Width");
            if (writeableBitmap.PixelHeight != image.Height) throw new ArgumentException("Different Height");
            if (writeableBitmap.GetBytesPerPixel() != image.GetBytesPerPixel()) throw new ArgumentException("Different BytesPerPixel");

            try
            {
                writeableBitmap.Lock();
                CopyImagePixelsImpl(image, writeableBitmap.BackBuffer, writeableBitmap.BackBufferStride);
            }
            finally
            {
                writeableBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight));
                writeableBitmap.Unlock();
            }

            //writeableBitmap.Freeze();
        }

        /// <summary>System.Windows.Media.Imaging.WriteableBitmap を新たに作成します</summary>
        public static System.Windows.Media.Imaging.WriteableBitmap ToWriteableBitmap(this Image<Bgr24> image)
        {
            if (image.IsInvalid()) throw new ArgumentException("Invalid Image");
            if (image.GetBytesPerPixel() != 3) throw new NotSupportedException("BytesPerPixel");

            var writeableBitmap = new System.Windows.Media.Imaging.WriteableBitmap(
                image.Width, image.Height,
                image.Metadata.HorizontalResolution, image.Metadata.VerticalResolution,
                System.Windows.Media.PixelFormats.Bgr24, null);

            CopyToWriteableBitmap(image, writeableBitmap);

            //writeableBitmap.Freeze();
            return writeableBitmap;
        }
        #endregion

        #region ToImagePixels
        /// <summary>ImagePixels に画素値をコピーします</summary>
        public static void CopyToImagePixels(this Image<Bgr24> image, ref ImagePixels pixels)
        {
            if (image.IsInvalid()) throw new ArgumentException("Invalid Bitmap");
            if (pixels.IsInvalid) throw new ArgumentException("Invalid Pixels");
            if (image.Width != pixels.Width) throw new ArgumentException("Different Width");
            if (image.Height != pixels.Height) throw new ArgumentException("Different Height");
            if (image.GetBytesPerPixel() != pixels.BytesPerPixel) throw new ArgumentException("Invalid BytesPerPixel");

            CopyImagePixelsImpl(image, pixels.PixelsPtr, pixels.Stride);
        }

        /// <summary>ImagePixelsContainer を作成して返します</summary>
        public static ImagePixelsContainer ToImagePixelsContainer(this Image<Bgr24> image)
        {
            if (image.IsInvalid()) throw new ArgumentException("Invalid Image");

            int size = Unsafe.SizeOf<Bgr24>();
            var container = new ImagePixelsContainer(image.Width, image.Height, size);
            var pixels = container.Pixels;
            CopyToImagePixels(image, ref pixels);

            return container;
        }
        #endregion

        /// <summary>Imageの画素値を書き出します</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void CopyImagePixelsImpl(Image<Bgr24> image, IntPtr destHeadPtr, int destStride)
        {
            var height = image.Height;
            var width = image.Width;
            var destRowPtr = (byte*)destHeadPtr;

            for (var y = 0; y < height; ++y, destRowPtr += destStride)
            {
                var pixelRowSpan = image.GetPixelRowSpan(y);
                var dest = (Bgr24*)destRowPtr;

                for (var x = 0; x < width; ++x)
                {
                    *(dest++) = pixelRowSpan[x];
                }
            }
        }
    }
}
