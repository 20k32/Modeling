using Modeling.Core.Drawing;
using Modeling.Models.Drawing.Figures;
using Modeling.Models.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Models.Abstractions.Drawing.Figure
{
    public interface IFigure : IEnumerable<IPointGeometry>
    {
        LinkedList<IPointGeometry> Segments { get; }

        void AddSegments(IEnumerable<IPointGeometry> segments);
        void AddSegment(IPointGeometry segment);
        void Clear();
        IPointGeometry GetFirstMatchingSegment(PointSingle point, float desiredPointDistance = Constants.DISTANCE_BETWEEN_SEGMENT_POINT_AND_USER_POINT_PIXELS);
    }
}
