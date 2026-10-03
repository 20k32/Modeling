using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.Drawing;
using Modeling.Core.Enums;
using System;

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

            return point.X >= bounds.Left - minDimensionSizeScaled &&
                   point.X <= bounds.Left + MathF.Max(bounds.Width, minDimensionSizeScaled) &&
                   point.Y >= bounds.Top - minDimensionSizeScaled &&
                   point.Y <= bounds.Top + MathF.Max(bounds.Height, minDimensionSizeScaled);
        }

        public static bool IntersectsBounds(this RectangleSingle first, RectangleSingle second, float pixelsPerCentimeter)
        {
            var minDimensionSizeScaled =
                MIN_BOUND_DIMENSION_SIZE_PIXELS / 10f * pixelsPerCentimeter;

            var firstRight = first.Left +
                             MathF.Max(first.Width, minDimensionSizeScaled);

            var firstBottom = first.Top +
                              MathF.Max(first.Height, minDimensionSizeScaled);

            var secondRight = second.Left +
                              MathF.Max(second.Width, minDimensionSizeScaled);

            var secondBottom = second.Top +
                               MathF.Max(second.Height, minDimensionSizeScaled);

            return first.Left <= secondRight &&
                   firstRight >= second.Left &&
                   first.Top <= secondBottom &&
                   firstBottom >= second.Top;
        }

        public static AdjacentType DetermineAdjacentType(this RectangleSingle first, RectangleSingle second, float pixelsPerCentimeter)
        {
            var result = AdjacentType.None;

            var minSize = MIN_BOUND_DIMENSION_SIZE_PIXELS / 10f * pixelsPerCentimeter;

            var firstLeft = first.Left;
            var firstTop = first.Top;
            var firstRight = firstLeft + MathF.Max(first.Width, minSize);
            var firstBottom = firstTop + MathF.Max(first.Height, minSize);

            var secondLeft = second.Left;
            var secondTop = second.Top;
            var secondRight = secondLeft + MathF.Max(second.Width, minSize);
            var secondBottom = secondTop + MathF.Max(second.Height, minSize);

            var intersects = firstLeft <= secondRight &&
                             firstRight >= secondLeft &&
                             firstTop <= secondBottom &&
                             firstBottom >= secondTop;

            if (!intersects)
            {
                return result;
            }

            if (firstLeft >= secondLeft && firstRight <= secondRight && // 1st is subset
                    firstTop >= secondTop && firstBottom <= secondBottom)
            {
                result = AdjacentType.Inner;
            }
            else if (secondLeft >= firstLeft && secondRight <= firstRight && // 1st superset
                    secondTop >= firstTop && secondBottom <= firstBottom)
            {
                result = AdjacentType.Outer;
            }
            else
            {
                result = AdjacentType.Nearby;
            }

            return result;
        }
    }
}
