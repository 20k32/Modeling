using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml.Data;
using Modeling.Core.Abstractions.Converters.PrimitivesConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.UI.Resources.Converters
{
    sealed class FloatScaleConverter : IValueConverter
    {
        const float DEFAULT_SCALE_FACTOR = 1f;

        IFloatScaleConverter _floatScaleConverter;

        public FloatScaleConverter()
        {
            _floatScaleConverter = Ioc.Default.GetRequiredService<IFloatScaleConverter>();
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var result = value;

            if (value is float valueToScale)
            {
                var scaleFactor = parameter is float parameterScaleFactor
                    ? parameterScaleFactor
                    : parameter is string parameterScaleFactorString
                      && float.TryParse(parameterScaleFactorString, out var parameterScaleFactorParsed)
                    ? parameterScaleFactorParsed
                    : DEFAULT_SCALE_FACTOR;

                result = _floatScaleConverter.Convert(valueToScale, scaleFactor);
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
