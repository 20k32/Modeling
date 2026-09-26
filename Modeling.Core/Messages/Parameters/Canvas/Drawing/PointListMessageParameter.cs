using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Modeling.Core.Messages.Parameters.Canvas.Drawing
{
    public class PointListMessageParameter(IReadOnlyList<PointSingle> points, DrawingColor color, bool shouldFillGeometry = false, DrawingColor fillColor = default, bool clearBeforeRedraw = false, DrawingColor backgroundColor = default, float thickness = DrawingConstants.GRID_DRAWING_THICKNESS, IObjectTree parent = default)
        : DrawingMessageParameter(clearBeforeRedraw, parent)
    {
        public IReadOnlyList<PointSingle> Points { get; init; } = points;
        public float Thickness { get; init; } = thickness;
        public DrawingColor Color { get; init; } = color;
        public DrawingColor BackgroundColor { get; init; } = backgroundColor;
        public bool ShouldFillGeometry { get; init; } = shouldFillGeometry;
        public DrawingColor FillColor { get; init; } = fillColor;

        public override bool IsDefault() => Color.IsDefault()
            || Points is null
            || Points.All(point => point == DrawingConstants.DEFAULT_POINT)
            || (ClearBeforeRedraw && BackgroundColor.IsDefault())
            || (ShouldFillGeometry && FillColor.IsDefault());
    }
}
