using Modeling.Core.Enums;
using Modeling.Models.Drawing.Figures.PointGeometries;

namespace Modeling.Models.Abstractions.Drawing.Figure
{
    public interface IAdjacentPointGeometry
    {
        IPointGeometry Geometry { get; set; }
        AdjacentType AdjacentType { get; set; }
    }
}
