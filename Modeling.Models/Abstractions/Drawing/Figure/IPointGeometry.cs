using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.CoreDelegates;
using Modeling.Core.Drawing;
using Modeling.Models.Abstractions.Drawing;
using Modeling.Models.Drawing.Figures.PointGeometries.Enums;
using Modeling.Models.Miscellaneous;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.Figures.PointGeometries
{
    public interface IPointGeometry : IBounds
    {
        event ActionEventHandler PointGeometryPropertyChanged;

        SegmentDimensionParameter SegmentDimension { get; set;  }

        DimensionType DimensionType { get; }
        GeometryType GeometryType { get; }

        IPointHashSetCollection DefaultPoints { get; }
        IPointHashSetCollection Points { get; }

        bool ContainsPoint(PointSingle point);
        void AddPoint(PointSingle point);
        void AddPointsRange(IEnumerable<PointSingle> points);
        void ClearPoints();

        void CommitPropertyChanges();
        void SetDefaultProperties();
    }
}
