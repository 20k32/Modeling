using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Collections;
using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.Drawing;
using System.Linq;

namespace Modeling.Models.Drawing.DrawingMessageValues.Points
{
    public sealed class DrawTransformedPointsMessageValue : DrawPointsMessageValue
    {
        public Matrix3x3Single Transform { get; init; }

        public DrawTransformedPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor, bool shouldFillGeometry, DrawingColor fillColor, Matrix3x3Single transform, params PointSingle[] points)
            : base(color, thickness, shouldClearCanvas, backgroundColor, shouldFillGeometry, fillColor, points)
        {
            var pointCollection = Ioc.Default.GetRequiredService<IPointListCollection>();

            pointCollection.AddRange(points ?? Enumerable.Empty<PointSingle>());

            Transform = transform;
        }

        public DrawTransformedPointsMessageValue(DrawingColor color, float thickness, bool shouldClearCanvas, DrawingColor backgroundColor, bool shouldFillGeometry, DrawingColor fillColor, Matrix3x3Single transform, IBlockingCollection<PointSingle> points)
            : base(color, thickness, shouldClearCanvas, backgroundColor, shouldFillGeometry, fillColor, points)
        {
            Transform = transform;
        }
    }
}
