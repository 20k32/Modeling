using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Models.Drawing.Figures.PointGeometries.Abstractions
{
    public interface ICirclePointGeometry : IPointGeometry
    {
        public float Diameter { get; set; }
    }
}
