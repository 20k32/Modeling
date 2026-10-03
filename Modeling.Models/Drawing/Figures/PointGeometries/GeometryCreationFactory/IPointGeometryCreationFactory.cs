using Modeling.Core.Drawing;
using Modeling.Models.Abstractions.Drawing.Figure;

namespace Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory
{
    public interface IPointGeometryCreationFactory
    {
        IPointGeometry CreateCirclePointGeometry(PointSingle centerCirclePoint, float startAngle = 0, float endAngle = 360);
        IPointGeometry CreateLinePointGeometry();
    }
}
