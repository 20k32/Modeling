using Modeling.Core.Drawing;
using Modeling.Models.Abstractions.Drawing.Figure;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.Figures
{
    public sealed class PointGeometry : IPointGeometry
    {
        public HashSet<PointSingle> Points { get; } = [];

        public bool TryAddPoint(PointSingle point) => Points.Add(point);
        public void AddPointsRange(IEnumerable<PointSingle> points)
        {
            foreach (var point in points)
            {
                _ = TryAddPoint(point);
            }
        }
    }
}
