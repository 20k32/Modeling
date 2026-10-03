using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Drawing;
using Modeling.Models.Abstractions.Drawing.Figure;

namespace Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory
{
    sealed class PointGeometryCreationFactory : IPointGeometryCreationFactory
    {
        public IPointGeometry CreateCirclePointGeometry(PointSingle centerCirclePoint, float startAngle = 0, float endAngle = 360)
        {
            var circlePointGeometry = Ioc.Default.GetRequiredService<CirclePointGeometry>();

            circlePointGeometry.StartAngle = startAngle;
            circlePointGeometry.EndAngle = endAngle;
            circlePointGeometry.CenterCirclePoint = centerCirclePoint;

            return circlePointGeometry;
        }

        public IPointGeometry CreateLinePointGeometry() => Ioc.Default.GetRequiredService<LinePointGeometry>();
    }
}
