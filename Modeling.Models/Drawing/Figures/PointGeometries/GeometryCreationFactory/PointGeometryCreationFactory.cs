using CommunityToolkit.Mvvm.DependencyInjection;

namespace Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory
{
    sealed class PointGeometryCreationFactory : IPointGeometryCreationFactory
    {
        public IPointGeometry CreateCirclePointGeometry() => Ioc.Default.GetRequiredService<CirclePointGeometry>();

        public IPointGeometry CreateLinePointGeometry() => Ioc.Default.GetRequiredService<LinePointGeometry>();
    }
}
