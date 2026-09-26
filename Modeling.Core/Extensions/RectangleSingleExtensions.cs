using Modeling.Core.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Core.Extensions
{
    public static class RectangleSingleExtensions
    {
        public static IEnumerable<PointSingle> GetPointsFromBounds(this RectangleSingle rectangle) =>
            [new(rectangle.Left, rectangle.Top), new(rectangle.Width, rectangle.Top), new(rectangle.Width, rectangle.Height), new(rectangle.Left, rectangle.Height), new(rectangle.Left, rectangle.Top)];
    }
}
