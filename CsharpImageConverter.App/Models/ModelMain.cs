using CsharpImageConverter.App.Common;
using Reactive.Bindings;
using System;
using System.Windows.Media.Imaging;

namespace CsharpImageConverter.App.Models
{
    class ModelMain : MyBindableBase
    {
        public IReactiveProperty<BitmapSource> ImageSource { get; }

        public ModelMain()
        {
            ImageSource = new ReactivePropertySlim<BitmapSource>();

            var bitmapSource = (BitmapSource)BitmapSourceExtension.FromFile(@"Assets\image0.jpg");

            //BitmapSourceExtension.ToPngFile(bitmapSource, @"_image.png");


            ImageSource.Value = bitmapSource;
        }
    }
}
