using Modeling.Core.Abstractions;
using Modeling.Core.Abstractions.Collections;
using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Enums;

namespace Modeling.Core.Messages.Parameters.Canvas.Drawing
{
    public sealed class PointListTransformMessageParameter(IBlockingCollection<PointSingle> points, DrawingColor color, bool applyAliasing, DrawingAliasing drawingAliasing, Matrix3x3Single transformMatrix, bool shouldFillGeometry = false, DrawingColor fillColor = default, bool clearBeforeRedraw = false, DrawingColor backgroundColor = default, float thickness = DrawingConstants.GRID_DRAWING_THICKNESS, IObjectTree parent = default)
        : PointListMessageParameter(points, color, applyAliasing, drawingAliasing, shouldFillGeometry, fillColor, clearBeforeRedraw, backgroundColor, thickness, parent)
    {
        public Matrix3x3Single TransformMatrix { get; init; } = transformMatrix;
    }
}
