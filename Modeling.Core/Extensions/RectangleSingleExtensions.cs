using Modeling.Core.Drawing;
using System.Collections.Generic;

namespace Modeling.Core.Extensions
{
    public static class RectangleSingleExtensions
    {
        public static IEnumerable<PointSingle> GetPointsFromBounds(this RectangleSingle rectangle) =>
            [new(rectangle.Left, rectangle.Top), new(rectangle.Right, rectangle.Top), new(rectangle.Right, rectangle.Bottom), new(rectangle.Left, rectangle.Bottom), new(rectangle.Left, rectangle.Top)];

        public static bool Contains(this RectangleSingle bounds, PointSingle point)
        {
            return point.X >= bounds.Left &&
                   point.X <= bounds.Left + bounds.Right &&
                   point.Y >= bounds.Top &&
                   point.Y <= bounds.Top + bounds.Bottom;
        }
    }
}
