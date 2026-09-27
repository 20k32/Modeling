using Modeling.Core.Drawing;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.DrawingMessageValues.Points
{
    public sealed class DrawTransformedPointsMessageValue : DrawPointsMessageValue
    {
        public Matrix3x3Single Transform { get; init; }

        public DrawTransformedPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor, bool shouldFillGeometry, DrawingColor fillColor, Matrix3x3Single transform, params PointSingle[] points)
            : base(color, thickness, shouldClearCanvas, backgroundColor, shouldFillGeometry, fillColor, points)
        {
            Transform = transform;
        }

        public DrawTransformedPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor, bool shouldFillGeometry, DrawingColor fillColor, Matrix3x3Single transform, IReadOnlyList<PointSingle> points)
            : base(color, thickness, shouldClearCanvas, backgroundColor, shouldFillGeometry, fillColor, points)
        {
            Transform = transform;
        }
    }
}
