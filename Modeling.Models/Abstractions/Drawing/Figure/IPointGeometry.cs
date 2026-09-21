using Modeling.Core.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Models.Abstractions.Drawing.Figure
{
    public interface IPointGeometry
    {
        HashSet<PointSingle> Points { get; }
        bool TryAddPoint(PointSingle point);

        void AddPointsRange(IEnumerable<PointSingle> points);
    }
}
