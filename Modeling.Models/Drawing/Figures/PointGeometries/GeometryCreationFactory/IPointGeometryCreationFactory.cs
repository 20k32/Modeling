using Modeling.Core.Drawing;
using Modeling.Models.Miscellaneous;

namespace Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory
{
    public interface IPointGeometryCreationFactory
    {
        IPointGeometry CreateCirclePointGeometry(PointSingle centerCirclePoint, SegmentDimensionParameter segmentDimension, float startAngle = 0, float endAngle = 360);
        IPointGeometry CreateLinePointGeometry(SegmentDimensionParameter segmentDimension);
    }
}
