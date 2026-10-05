using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Drawing;
using Modeling.Models.Miscellaneous;

namespace Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory
{
    sealed class PointGeometryCreationFactory : IPointGeometryCreationFactory
    {
        public IPointGeometry CreateCirclePointGeometry(PointSingle centerCirclePoint, SegmentDimensionParameter segmentDimension, float startAngle = 0, float endAngle = 360)
        {
            var circlePointGeometry = Ioc.Default.GetRequiredService<CirclePointGeometry>();

            circlePointGeometry.SegmentDimension = segmentDimension;

            circlePointGeometry.StartAngle = startAngle;
            circlePointGeometry.EndAngle = endAngle;
            circlePointGeometry.CenterCirclePoint = centerCirclePoint;

            return circlePointGeometry;
        }

        public IPointGeometry CreateLinePointGeometry(SegmentDimensionParameter segmentDimension)
        {
            var lineGeometry = Ioc.Default.GetRequiredService<LinePointGeometry>();
            lineGeometry.SegmentDimension = segmentDimension;

            return lineGeometry;
        }
    }
}
