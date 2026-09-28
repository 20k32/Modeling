using Modeling.Core.CoreDelegates;
using Modeling.Core.Drawing;
using Modeling.Models.Abstractions.Drawing;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.Figures.PointGeometries
{
    public interface IPointGeometry : IBounds
    {
        event ActionEventHandler PointGeometryPropertyChanged;

        GeometryType GeometryType { get; }
        HashSet<PointSingle> Points { get; }
        bool TryAddPoint(PointSingle point);
        void AddPointsRange(IEnumerable<PointSingle> points);
    }
}
