using Modeling.Core.Drawing;
using Modeling.Models.Enums;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.DrawingMessageValues
{
    public class TransformPointsMessageValue : DrawingMessageValue
    {
        public IReadOnlyList<PointSingle> Points { get; init; }
        public float Thickness { get; init; }
        public Matrix3x3Single Transform { get; init; }
        public DrawingColor BackgroundColor { get; init; }
        public bool ShouldClearBeforeRedraw { get; init; }

        TransformPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor) : base(color)
        {
            MessageType = DrawingMessageType.Transform;
            Thickness = thickness;
            ShouldClearBeforeRedraw = shouldClearCanvas;
            BackgroundColor = backgroundColor;
        }

        public TransformPointsMessageValue(DrawingColor color, float thickness, Matrix3x3Single transform, bool shouldClearCanvas, DrawingColor backgroundColor, params PointSingle[] points)
            : this(color, thickness, shouldClearCanvas, backgroundColor)
        {
            Points = points ?? [];
            Transform = transform;
        }

        public TransformPointsMessageValue(DrawingColor color, float thickness, Matrix3x3Single transform, bool shouldClearCanvas, DrawingColor backgroundColor, IReadOnlyList<PointSingle> points)
            : this(color, thickness, shouldClearCanvas, backgroundColor)
        {
            Points = points ?? [];
            Transform = transform;
        }

        public override bool IsDefault() => base.IsDefault() || (Points?.Count ?? 0) == 0;
    }
}
