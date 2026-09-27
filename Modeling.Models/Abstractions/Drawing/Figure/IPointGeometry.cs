using Modeling.Core.Drawing;
using System.Collections.Generic;

namespace Modeling.Models.Abstractions.Drawing.Figure
{
    public interface IPointGeometry : IBounds
    {
        HashSet<PointSingle> Points { get; }
        bool TryAddPoint(PointSingle point);
        void AddPointsRange(IEnumerable<PointSingle> points);
    }
}
