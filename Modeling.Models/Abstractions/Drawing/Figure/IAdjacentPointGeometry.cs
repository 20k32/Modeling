using Modeling.Models.Enums;

namespace Modeling.Models.Abstractions.Drawing.Figure
{
    public interface IAdjacentPointGeometry
    {
        IPointGeometry Geometry { get; set; }
        AdjacentType AdjacentType { get; set; }
    }
}
