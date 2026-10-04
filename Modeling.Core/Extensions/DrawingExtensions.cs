using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using System;
using System.Collections.Generic;

namespace Modeling.Core.Extensions
{
    public static class DrawingExtensions
    {
        public static IList<PointSingle> GetCirclePoints(this PointSingle centerCoordinate, float radius,
            float startAngle = DrawingConstants.CIRCLE_START_ANGLE_DEGREES, float endAngle = DrawingConstants.CIRCLE_END_ANGLE_DEGREES)
        {
            var result = new List<PointSingle>();

            for (var i = startAngle; i <= endAngle; i++)
            {
                var angle = i.DegreesToRadian();

                var x = centerCoordinate.X + radius * MathF.Cos(angle);
                var y = centerCoordinate.Y + radius * MathF.Sin(angle);

                result.Add(new PointSingle(x, y));
            }

            return result;
        }
    }
}
