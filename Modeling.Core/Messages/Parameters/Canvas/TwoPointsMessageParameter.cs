using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;

namespace Modeling.Core.Messages.Parameters.Canvas
{
    public sealed class TwoPointsMessageParameter : IDefaultCheck
    {
        public readonly PointSingle PointA;
        public readonly PointSingle PointB;

        public readonly float Thickness;
        public DrawingColor Color;


        public TwoPointsMessageParameter(PointSingle pointA, PointSingle pointB, DrawingColor color, float thickness = DrawingConstants.DEFAULT_DRAWING_THICKNESS)
        {
            PointA = pointA;
            PointB = pointB;
            Color = color;
            Thickness = thickness;
        }

        public bool IsDefault() => PointA == DrawingConstants.DEFAULT_POINT && PointB == DrawingConstants.DEFAULT_POINT;
    }
}
