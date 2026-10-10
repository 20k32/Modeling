using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Collections;
using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.Drawing;
using Modeling.Core.Enums;
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

        protected DrawPointsMessageValue(DrawingColor color, float thickness, DrawingColor backgroundColor, bool shouldClearCanvas, bool shouldFillGeometry, DrawingColor fillColor, bool shouldApplyAliasing, DrawingAliasing drawingAliasing) : base(backgroundColor, shouldClearCanvas, shouldApplyAliasing, drawingAliasing)
        {
            Color = color;
            ShouldClearBeforeRedraw = shouldClearCanvas;
            Thickness = thickness;
            ShouldFillGeometry = shouldFillGeometry;
            FillColor = fillColor;
        }

        public DrawPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor, bool shouldFillGeometry, DrawingColor fillColor, bool shouldApplyAliasing, DrawingAliasing drawingAliasing, params PointSingle[] points)
            : this(color, thickness, backgroundColor, shouldClearCanvas, shouldFillGeometry, fillColor, shouldApplyAliasing, drawingAliasing)
        {
            var pointsCollection = Ioc.Default.GetRequiredService<IPointListCollection>();

            pointsCollection.AddRange(points ?? Enumerable.Empty<PointSingle>());

            Points = pointsCollection;
        }

        public DrawPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor, bool shouldFillGeometry, DrawingColor fillColor, bool shouldApplyAliasing, DrawingAliasing drawingAliasing, IBlockingCollection<PointSingle> points)
            : this(color, thickness, backgroundColor, shouldClearCanvas, shouldFillGeometry, fillColor, shouldApplyAliasing, drawingAliasing)
        {
            Points = points;
        }
    }
}
