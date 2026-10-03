using Modeling.Core.Drawing;
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

        public float StartAngle { get; set;  }

        public float EndAngle { get; set; }

        public PointSingle CenterCirclePoint { get; set; }

        //MathF.Max used there because Circle also can be semicircle (one dimension bigger than another).
        protected override float CalculateDistance() => MathF.Max(Bounds.Width, Bounds.Height);

        protected override PointSingle CalculateCenterPointCore() => CenterCirclePoint;
    }
}
