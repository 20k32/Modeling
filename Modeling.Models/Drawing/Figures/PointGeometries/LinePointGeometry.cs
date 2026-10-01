using Modeling.Models.Drawing.Figures.PointGeometries.Abstractions;
using Modeling.Models.Drawing.Figures.PointGeometries.Enums;

namespace Modeling.Models.Drawing.Figures.PointGeometries
{
    sealed class LinePointGeometry : PointGeometry, ILinePointGeometry
    {
        public override DimensionType DimensionType { get; protected set; } = DimensionType.Length;
        public override GeometryType GeometryType { get; protected set; } = GeometryType.Line;

        public float Length
        {
            get => Distance;
            set => Distance = value;
        }
    }
}
