namespace Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory
{
    public interface IPointGeometryCreationFactory
    {
        IPointGeometry CreateCirclePointGeometry();
        IPointGeometry CreateLinePointGeometry();
    }
}
