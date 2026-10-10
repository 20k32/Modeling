using Microsoft.UI.Xaml.Data;
using System;

namespace Modeling.UI.Resources.Converters
{
    sealed class InvertFloatConverter : IValueConverter
    {
        const float INVERT_COEFFICIENT = -1;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var result = value;

            if (float.TryParse(value?.ToString() ?? string.Empty, out var actualValue))
            {
                result = actualValue * INVERT_COEFFICIENT;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
