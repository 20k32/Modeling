using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Core.Extensions
{
    public static class DrawingExtensions
    {
        private const float CIRCLE_START_ANGLE_DEGREES = 0f;
        private const float CIRCLE_END_ANGLE_DEGREES = 360;

        public static IList<PointSingle> GetCirclePoints(this PointSingle centerCoordinate, float radius,
            float startAngle = CIRCLE_START_ANGLE_DEGREES, float endAngle = CIRCLE_END_ANGLE_DEGREES)
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
