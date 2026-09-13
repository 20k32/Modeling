using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Modeling.Core.Messages.Parameters.Canvas
{
    public sealed class PointListMessageParameter(IList<PointSingle> points, DrawingColor color, float thickness = DrawingConstants.DEFAULT_DRAWING_THICKNESS)
        : IDefaultCheck
    {
        public IList<PointSingle> Points { get; init; } = points;
        public float Thickness { get; init; } = thickness;
        public DrawingColor Color { get; init; } = color;

        public bool IsDefault() => Color is null || Points is null || Points.All(point => point == DrawingConstants.DEFAULT_POINT);
    }
}
