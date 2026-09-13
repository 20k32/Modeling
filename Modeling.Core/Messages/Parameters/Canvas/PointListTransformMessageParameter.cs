using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Modeling.Core.Messages.Parameters.Canvas
{
    public sealed class PointListTransformMessageParameter(IReadOnlyList<PointSingle> points, DrawingColor color, Matrix3x3Single transformMatrix, bool clearBeforeRedraw = false, DrawingColor backgroundColor = default, float thickness = DrawingConstants.DEFAULT_DRAWING_THICKNESS)
        : IDefaultCheck
    {
        public IReadOnlyList<PointSingle> Points { get; init; } = points;
        public float Thickness { get; init; } = thickness;
        public DrawingColor Color { get; init; } = color;
        public Matrix3x3Single TransformMatrix { get; init; } = transformMatrix;
        public bool ShouldClearBeforeRedraw { get; init; } = clearBeforeRedraw;
        public DrawingColor BackgroundColor { get; init; } = backgroundColor;

        public bool IsDefault() => (Color?.IsDefault() ?? true)
            || Points is null
            || Points.All(point => point == DrawingConstants.DEFAULT_POINT)
            || (ShouldClearBeforeRedraw && (BackgroundColor?.IsDefault() ?? true));
    }
}
