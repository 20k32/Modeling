using Modeling.Models.Drawing.Figures.PointGeometries.Abstractions;

namespace Modeling.Models.Drawing.Figures.PointGeometries
{
    sealed class CirclePointGeometry : PointGeometry, ICirclePointGeometry
    {
        public override GeometryType GeometryType { get; protected set; } = GeometryType.Circle;

        public float Diameter
        {
            get => Distance;
            set => Distance = value;
        }
    }
}
