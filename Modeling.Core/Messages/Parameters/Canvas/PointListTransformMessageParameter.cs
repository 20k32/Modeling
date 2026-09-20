using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Messages.Canvas.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Modeling.Core.Messages.Parameters.Canvas
{
    public sealed class PointListTransformMessageParameter(IReadOnlyList<PointSingle> points, DrawingColor color, Matrix3x3Single transformMatrix, bool clearBeforeRedraw = false, DrawingColor backgroundColor = default, float thickness = DrawingConstants.DEFAULT_DRAWING_THICKNESS, IObjectTree parent = default)
        : PointListMessageParameter(points, color, clearBeforeRedraw, backgroundColor, thickness, parent)
    {
        public Matrix3x3Single TransformMatrix { get; init; } = transformMatrix;
    }
}
