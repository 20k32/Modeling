using Modeling.Models.Drawing.Figures.PointGeometries.Abstractions;
using Modeling.Models.Drawing.Figures.PointGeometries.Enums;
using System;

namespace Modeling.Models.Drawing.Figures.PointGeometries
{
    sealed class CirclePointGeometry : PointGeometry, ICirclePointGeometry
    {
        public override GeometryType GeometryType { get; protected set; } = GeometryType.Circle;
        public override DimensionType DimensionType { get; protected set; } = DimensionType.Radius;

        public float Diameter
        {
            get => Distance;
            set => Distance = value;
        }

        protected override float CalculateDistance() => MathF.Min(Bounds.Width, Bounds.Height);
    }
}
