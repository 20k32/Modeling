using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Collections;
using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Modeling.Models.Drawing.DrawingMessageValues.Points
{
    public class DrawPointsMessageValue : DrawMessageValue
    {
        public DrawingColor Color { get; init; }
        public IBlockingCollection<PointSingle> Points { get; init; }
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
            var pointsCollection = Ioc.Default.GetRequiredService<IPointListCollection>();

            pointsCollection.AddRange(points ?? Enumerable.Empty<PointSingle>());

            Points = pointsCollection;
        }

        public DrawPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor, bool shouldFillGeometry, DrawingColor fillColor, IBlockingCollection<PointSingle> points)
            : this(color, thickness, backgroundColor, shouldClearCanvas, shouldFillGeometry, fillColor)
        {
            Points = points;
        }
    }
}
