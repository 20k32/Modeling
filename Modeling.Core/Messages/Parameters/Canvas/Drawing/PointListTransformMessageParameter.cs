using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using System.Collections.Generic;

namespace Modeling.Core.Messages.Parameters.Canvas.Drawing
{
    public sealed class PointListTransformMessageParameter(IReadOnlyList<PointSingle> points, DrawingColor color, Matrix3x3Single transformMatrix, bool shouldFillGeometry = false, DrawingColor fillColor = default, bool clearBeforeRedraw = false, DrawingColor backgroundColor = default, float thickness = DrawingConstants.GRID_DRAWING_THICKNESS, IObjectTree parent = default)
        : PointListMessageParameter(points, color, shouldFillGeometry, fillColor, clearBeforeRedraw, backgroundColor, thickness, parent)
    {
        public Matrix3x3Single TransformMatrix { get; init; } = transformMatrix;
    }
}
