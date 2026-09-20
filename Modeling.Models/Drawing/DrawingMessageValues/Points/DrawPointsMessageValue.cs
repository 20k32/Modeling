using Modeling.Core.Drawing;
using Modeling.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Models.Drawing.DrawingMessageValues.Points
{
    public class DrawPointsMessageValue : DrawMessageValue
    {
        public DrawingColor Color { get; init; }
        public IReadOnlyList<PointSingle> Points { get; init; }
        public float Thickness { get; init; }

        protected DrawPointsMessageValue(DrawingColor color, float thickness, DrawingColor backgroundColor, bool shouldClearCanvas) : base(backgroundColor, shouldClearCanvas)
        {
            Color = color;
            ShouldClearBeforeRedraw = shouldClearCanvas;
            Thickness = thickness;
        }

        public DrawPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor, params PointSingle[] points)
            : this(color, thickness, backgroundColor, shouldClearCanvas)
        {
            Points = points ?? [];
        }

        public DrawPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor, IReadOnlyList<PointSingle> points)
            : this(color, thickness, backgroundColor, shouldClearCanvas)
        {
            Points = points ?? [];
        }
    }
}
