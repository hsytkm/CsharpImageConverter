using System;
using System.Collections.Generic;
using System.Text;

namespace BitmapPixels
{
    static class ImageSharpExtension
    {
#if false
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

        private ImageContainer(Stream stream, IImageDecoder decoder)
        {
            ImageFileName = "FromStream(Unknown)";  // 何か入れとく

            // 以下がないとBMPの読出しに失敗した!!
            stream.Position = 0;

            using (var image = Image.Load<Gray8>(stream, decoder))
            {
                ImagePayload = ReadImagePayload(image);
            }
        }

        public static ImageContainer GetInstanceFromStream(Stream stream) =>
            GetInstanceFromBitmap(stream);

        private static ImageContainer GetInstanceFromJpeg(Stream stream) =>
            new ImageContainer(stream, new JpegDecoder());

        private static ImageContainer GetInstanceFromBitmap(Stream stream) =>
            new ImageContainer(stream, new BmpDecoder());

        private static ImageContainer GetInstanceFromPng(Stream stream)
        {
            var decoder = new PngDecoder { IgnoreMetadata = true };
            return new ImageContainer(stream, decoder);
        }

        private ImagePayload ReadImagePayload(Image<Gray8> image)
        {
            int i = 0;
            var pixels = new byte[image.Width * image.Height];
            for (int y = 0; y < image.Height; y++)
            {
                foreach (var gray8 in image.GetPixelRowSpan(y))
                {
                    pixels[i++] = gray8.PackedValue;
                }
            }

            UnmanagedPointer = Marshal.AllocHGlobal(pixels.Length);
            Marshal.Copy(pixels, 0, UnmanagedPointer, pixels.Length);

            var bytesPerPixel = (image.PixelType.BitsPerPixel + (8 - 1)) / 8;
            return new ImagePayload(
                image.Width,
                image.Height,
                bytesPerPixel,
                image.Width * bytesPerPixel,
                UnmanagedPointer);
        }


#endif
    }
}
