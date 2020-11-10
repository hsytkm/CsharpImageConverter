using CsharpImageConverter.App.Common;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CsharpImageConverter.App.ViewModels
{
    class ConvertFlowViewModel : MyBindableBase
    {
        public IList<ImageFileFormat> InputImageFormatItems { get; }
            = EnumExtension.GetItems<ImageFileFormat>().ToList();
        public IReactiveProperty<ImageFileFormat> SelectedInputImageFormat { get; }

        public IList<ImageClass> ConvertImageClassItems { get; }
            = EnumExtension.GetItems<ImageClass>().ToList();
        public IReactiveProperty<ImageClass> SelectedConvertImageClass { get; }


        public IList<ImageFileFormat> OutputImageFormatItems { get; }
            = EnumExtension.GetItems<ImageFileFormat>().ToList();
        public IReactiveProperty<ImageFileFormat> SelectedOutputImageFormat { get; }

        public ConvertFlowViewModel()
        {
            SelectedInputImageFormat = new ReactivePropertySlim<ImageFileFormat>();
            SelectedConvertImageClass = new ReactivePropertySlim<ImageClass>();
            SelectedOutputImageFormat = new ReactivePropertySlim<ImageFileFormat>();


            SelectedInputImageFormat
                .Subscribe(x => Debug.WriteLine($"SelectedSourceImageFormat: {x}"));

        }
        
    }


}
