using CsharpImageConverter.App.Common;
using CsharpImageConverter.Core;
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

            var bitmapSource = (BitmapSource)BitmapSourceExtension.FromFile(AppCommonSettings.JpegFilePath);

            //BitmapSourceExtension.ToPngFile(bitmapSource, AppCommonSettings.PngFilePath);


            ImageSource.Value = bitmapSource;
        }
    }
}
