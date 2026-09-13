using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;

namespace Modeling.Core.Messages.Parameters.Canvas
{
    public sealed class TwoPointsMessageParameter(PointSingle pointA, PointSingle pointB, DrawingColor color, bool clearBeforeRedraw = false, DrawingColor backgroundColor = default, float thickness = DrawingConstants.DEFAULT_DRAWING_THICKNESS) : IDefaultCheck
    {
        public PointSingle PointA { get; init; } = pointA;
        public PointSingle PointB { get; init; } = pointB;

        public float Thickness { get; init; } = thickness;
        public DrawingColor Color { get; init; } = color;

        public bool ShouldClearBeforeRedraw { get; init; } = clearBeforeRedraw;
        public DrawingColor BackgroundColor { get; init; } = backgroundColor;

        public bool IsDefault() => (Color?.IsDefault() ?? true)
            || PointA == DrawingConstants.INVALID_POINT
            || PointB == DrawingConstants.INVALID_POINT
            || (ShouldClearBeforeRedraw && (BackgroundColor?.IsDefault() ?? true));
    }
}
