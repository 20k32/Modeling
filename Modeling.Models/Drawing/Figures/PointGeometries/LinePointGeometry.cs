using Modeling.Models.Drawing.Figures.PointGeometries.Abstractions;

namespace Modeling.Models.Drawing.Figures.PointGeometries
{
    sealed class LinePointGeometry : PointGeometry, ILinePointGeometry
    {
        public override GeometryType GeometryType { get; protected set; } = GeometryType.Line;

        public float Length
        {
            get => Distance;
            set => Distance = value;
        }
    }
}
