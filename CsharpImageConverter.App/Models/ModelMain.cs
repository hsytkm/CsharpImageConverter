using CsharpImageConverter.App.Common;
using CsharpImageConverter.Core;
using Reactive.Bindings;
using System;
using System.Windows.Media.Imaging;

namespace CsharpImageConverter.App.Models
{
    class ModelMain : MyBindableBase
    {
        public IReactiveProperty<BitmapSource> ImageSource0 { get; }
            = new ReactivePropertySlim<BitmapSource>();

        public IReactiveProperty<BitmapSource> ImageSource1 { get; }
            = new ReactivePropertySlim<BitmapSource>();

        public ModelMain()
        {
            LoadImageFlow0(@"Assets\flow0.png");
            LoadImageFlow1(@"Assets\flow1.png");
        }

        private void LoadImageFlow0(string path)
        {
            ImageSource0.Value = BitmapSourceExtension.FromFile(path);
        }

        private void LoadImageFlow1(string path)
        {
            var bitmapSource0 = BitmapSourceExtension.FromFile(path);

            using var drawingBitmap1 = bitmapSource0.ToDrawingBitmap();
            using var container1 = drawingBitmap1.ToImagePixelsContainer();
            var pixels1 = container1.Pixels;
            using var image1 = pixels1.ToImageSharpBgr24();

            var bitmapSource2 = image1.ToBitmapSource();
            using var image2 = bitmapSource2.ToImageSharpBgr24();
            using var container2 = image2.ToImagePixelsContainer();
            var pixels2 = container2.Pixels;
            using var drawingBitmap2 = pixels2.ToDrawingBitmap();

            using var image3 = drawingBitmap2.ToImageSharpBgr24();
            using var drawingBitmap3 = image3.ToDrawingBitmap();
            var bitmapSource3 = drawingBitmap2.ToBitmapSource();

            using var container3 = bitmapSource3.ToImagePixelsContainer();
            var pixels3 = container3.Pixels;
            var bitmapSource4 = pixels3.ToBitmapSource();

            ImageSource1.Value = bitmapSource4;
        }
    }
}
