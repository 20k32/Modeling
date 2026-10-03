using Modeling.Models.Abstractions.Drawing.Figure;

namespace Modeling.Models.Drawing.Figures.PointGeometries.Abstractions
{
    public interface ILinePointGeometry : IPointGeometry
    {
        float Length { get; set; }
    }
}
