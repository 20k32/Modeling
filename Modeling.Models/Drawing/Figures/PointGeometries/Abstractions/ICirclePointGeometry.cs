using Modeling.Core.Drawing;
using Modeling.Models.Abstractions.Drawing.Figure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Models.Drawing.Figures.PointGeometries.Abstractions
{
    public interface ICirclePointGeometry : IPointGeometry
    {
        public float StartAngle { get; set; }
        public float EndAngle { get; set; }
        public float Diameter { get; set; }

        PointSingle CenterCirclePoint { get; set; }
    }
}
