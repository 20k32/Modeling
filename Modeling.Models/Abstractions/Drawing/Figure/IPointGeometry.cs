using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.CoreDelegates;
using Modeling.Core.Drawing;
using Modeling.Models.Abstractions.Drawing;
using Modeling.Models.Drawing.Figures.PointGeometries.Enums;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.Figures.PointGeometries
{
    public interface IPointGeometry : IBounds, IAdjacentGeometriesProvider
    {
        event ActionEventHandler PointGeometryPropertyChanged;

        DimensionType DimensionType { get; }
        GeometryType GeometryType { get; }

        IPointHashSetCollection DefaultPoints { get; }
        IPointHashSetCollection Points { get; }

        bool ContainsPoint(PointSingle point);
        void AddPoint(PointSingle point);
        void AddPointsRange(IEnumerable<PointSingle> points);
        void ClearPoints();

        void CommitPropertyChanges();
        void UpdateAdjacentGeometriesBounds();
        void SetDefaultProperties();
    }
}
