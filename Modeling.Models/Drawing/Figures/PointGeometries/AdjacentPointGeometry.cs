using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Enums;

namespace Modeling.Models.Drawing.Figures.PointGeometries
{
    sealed class AdjacentPointGeometry : IAdjacentPointGeometry
    {
        public IPointGeometry Geometry { get; set; }

        public AdjacentType AdjacentType { get; set; }
    }
}
