using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.Drawing;
using System;
using System.Collections.Generic;

namespace Modeling.Core.Extensions
{
    public static class RectangleSingleExtensions
    {
        private const float MIN_BOUND_DIMENSION_SIZE_PIXELS = 1.5f;
        public static IPointListCollection GetPointsFromBounds(this RectangleSingle rectangle)
        {
            var pointCollection = Ioc.Default.GetRequiredService<IPointListCollection>();
            pointCollection.AddRange([new(rectangle.Left, rectangle.Top), new(rectangle.Right, rectangle.Top), new(rectangle.Right, rectangle.Bottom), new(rectangle.Left, rectangle.Bottom), new(rectangle.Left, rectangle.Top)]);

            return pointCollection;
        }

        public static bool Contains(this RectangleSingle bounds, PointSingle point, float pixelsPerCentimeter)
        {
            var minDimensionSizeScaled = MIN_BOUND_DIMENSION_SIZE_PIXELS / 10 * pixelsPerCentimeter;

            return point.X >= bounds.Left &&
                   point.X <= bounds.Left + MathF.Max(bounds.Width, minDimensionSizeScaled) &&
                   point.Y >= bounds.Top - minDimensionSizeScaled &&
                   point.Y <= bounds.Top + MathF.Max(bounds.Height, minDimensionSizeScaled);
        }
    }
}
