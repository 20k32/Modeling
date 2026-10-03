using Modeling.Core.Drawing;
using Modeling.Models.Abstractions.Collections.Drawings;
using Modeling.Models.Drawing.Figures.PointGeometries;
using Modeling.Models.Miscellaneous;
using System.Collections.Generic;

namespace Modeling.Models.Abstractions.Drawing.Figure
{
    public interface IFigure : IEnumerable<IPointGeometry>, IBounds
    {
        IPointGeometryCollection Segments { get; }
        PointSingle CenterPoint { get; }

        void CalculatePropertiesFromSegments();
        void AddSegments(IEnumerable<IPointGeometry> segments);
        void AddSegment(IPointGeometry segment);
        void Clear();
        IPointGeometry GetFirstMatchingSegment(PointSingle point, float desiredPointDistance = Constants.DISTANCE_BETWEEN_SEGMENT_POINT_AND_USER_POINT_PIXELS);
    }
}
