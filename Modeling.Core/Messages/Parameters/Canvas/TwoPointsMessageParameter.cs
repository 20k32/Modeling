using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;

namespace Modeling.Core.Messages.Parameters.Canvas
{
    public sealed class TwoPointsMessageParameter(PointSingle pointA, PointSingle pointB, DrawingColor color, float thickness = DrawingConstants.DEFAULT_DRAWING_THICKNESS) : IDefaultCheck
    {
        public PointSingle PointA { get; init; } = pointA;
        public PointSingle PointB { get; init; } = pointB;

        public float Thickness { get; init; } = thickness;
        public DrawingColor Color { get; init; } = color;

        public bool IsDefault() => PointA == DrawingConstants.DEFAULT_POINT && PointB == DrawingConstants.DEFAULT_POINT;
    }
}
