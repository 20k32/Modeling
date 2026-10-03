using Modeling.Core.Enums;
using Modeling.Models.Abstractions.Drawing.Figure;

namespace Modeling.Models.Drawing.Figures.PointGeometries
{
    sealed class AdjacentPointGeometry : IAdjacentPointGeometry
    {
        public IPointGeometry Geometry { get; set; }

        public AdjacentType AdjacentType { get; set; }
    }
}
