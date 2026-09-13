using Modeling.Core.Drawing;
using Modeling.Models.Enums;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.DrawingMessageValues
{
    public sealed class ConnectPointsMessageValue : DrawingMessageValue
    {
        public IReadOnlyList<PointSingle> Points { get; init; }
        public float Thickness { get; init; }
        public DrawingColor BackgroundColor { get; init; }
        public bool ShouldClearBeforeRedraw { get; init; }

        ConnectPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor) : base(color)
        {
            MessageType = DrawingMessageType.Draw;
            ShouldClearBeforeRedraw = shouldClearCanvas;
            BackgroundColor = backgroundColor;
            Thickness = thickness;
        }

        public ConnectPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor, params PointSingle[] points)
            : this(color, thickness, shouldClearCanvas, backgroundColor)
        {
            Points = points ?? [];
        }

        public ConnectPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor, IReadOnlyList<PointSingle> points)
            : this(color, thickness, shouldClearCanvas, backgroundColor)
        {
            Points = points ?? [];
        }

        public override bool IsDefault() => base.IsDefault() 
            || (Points?.Count ?? 0) == 0 
            || (ShouldClearBeforeRedraw && (BackgroundColor?.IsDefault() ?? true));
    }
}
