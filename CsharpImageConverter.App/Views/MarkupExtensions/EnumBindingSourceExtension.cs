using System;
using System.Windows.Markup;

namespace CsharpImageConverter.App.Views.MarkupExtensions
{
    class EnumBindingSourceExtension : MarkupExtension
    {
        public Type EnumType { get; }

        public EnumBindingSourceExtension(Type type)
        {
            if (type is null || !type.IsEnum)
                throw new ArgumentException("type is must not be null and of type Enum");

            EnumType = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Enum.GetValues(EnumType);
        }
    }
}
