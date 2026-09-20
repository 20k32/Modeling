using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Modeling.Core.Messages.Parameters.Canvas
{
    public class PointListMessageParameter(IReadOnlyList<PointSingle> points, DrawingColor color, bool shouldFillGeometry = false, DrawingColor fillColor = default, bool clearBeforeRedraw = false, DrawingColor backgroundColor = default, float thickness = DrawingConstants.DEFAULT_DRAWING_THICKNESS, IObjectTree parent = default)
        : IDefaultCheck, IObjectTree
    {
        public IObjectTree Parent { get; init; } = parent;

        public IReadOnlyList<PointSingle> Points { get; init; } = points;
        public float Thickness { get; init; } = thickness;
        public DrawingColor Color { get; init; } = color;
        public bool ShouldClearBeforeRedraw { get; init; } = clearBeforeRedraw;
        public DrawingColor BackgroundColor { get; init; } = backgroundColor;
        public bool ShouldFillGeometry { get; init; } = shouldFillGeometry;
        public DrawingColor FillColor { get; init; } = fillColor;

        public bool IsDefault() => (Color?.IsDefault() ?? true)
            || Points is null
            || Points.All(point => point == DrawingConstants.DEFAULT_POINT)
            || (ShouldClearBeforeRedraw && (BackgroundColor?.IsDefault() ?? true))
            || (ShouldFillGeometry && (FillColor?.IsDefault() ?? true));
    }
}
