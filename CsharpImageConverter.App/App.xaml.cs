using CsharpImageConverter.App.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CsharpImageConverter.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static new App Current => (App)Application.Current;
        internal ModelMain ModelMain => new ModelMain();


    }
}
