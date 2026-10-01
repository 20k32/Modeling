using Microsoft.UI.Xaml.Data;
using Modeling.Core.Abstractions.Converters.PrimitivesConverters;
using Modeling.Core.Extensions;
using System;

namespace Modeling.UI.Resources.Converters
{
    sealed class FloatScaleConverter : IFloatScaleConverter
    {
        const float MINIMUM_SCALE_FACTOR = 0.01f;
        const float MAXIMUM_SCALE_FACTOR = 100f;
        const float DEFAULT_SCALE_FACTOR = 1;

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

                result = valueToScale * scaleFactor;
            }

            return result;
        }

        public float Convert(float source, float parameter)
        {
            parameter = MathFloatExtensions.Clamp(parameter, MINIMUM_SCALE_FACTOR, MAXIMUM_SCALE_FACTOR);
            return source * parameter;
        }

        public float ConvertBack(float destination, float parameter)
        {
            parameter = MathFloatExtensions.Clamp(parameter, MINIMUM_SCALE_FACTOR, MAXIMUM_SCALE_FACTOR);
            return destination / parameter;
        }
    }
}
