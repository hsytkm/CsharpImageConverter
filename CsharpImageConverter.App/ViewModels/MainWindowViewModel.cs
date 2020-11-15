using CsharpImageConverter.App.Common;
using CsharpImageConverter.App.Models;
using Reactive.Bindings;
using System;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace CsharpImageConverter.App.ViewModels
{
    class MainWindowViewModel : MyBindableBase
    {
        private readonly ModelMain _model = App.Current.ModelMain;

        public IReactiveProperty<BitmapSource> ImageSource0 { get; }
        public IReactiveProperty<BitmapSource> ImageSource1 { get; }

        public MainWindowViewModel()
        {
            ImageSource0 = _model.ImageSource0;
            ImageSource1 = _model.ImageSource1;
        }
    }
}
