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
        public bool ShouldFillGeometry { get; init; }
        public DrawingColor FillColor { get; init; }

        protected DrawPointsMessageValue(DrawingColor color, float thickness, DrawingColor backgroundColor, bool shouldClearCanvas, bool shouldFillGeometry, DrawingColor fillColor) : base(backgroundColor, shouldClearCanvas)
        {
            Color = color;
            ShouldClearBeforeRedraw = shouldClearCanvas;
            Thickness = thickness;
            ShouldFillGeometry = shouldFillGeometry;
            FillColor = fillColor;
        }

        public DrawPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor, bool shouldFillGeometry, DrawingColor fillColor, params PointSingle[] points)
            : this(color, thickness, backgroundColor, shouldClearCanvas, shouldFillGeometry, fillColor)
        {
            Points = points ?? [];
        }

        public DrawPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor, bool shouldFillGeometry, DrawingColor fillColor, IReadOnlyList<PointSingle> points)
            : this(color, thickness, backgroundColor, shouldClearCanvas, shouldFillGeometry, fillColor)
        {
            Points = points ?? [];
        }
    }
}
