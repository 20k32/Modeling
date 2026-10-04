using Modeling.Core.Drawing;
using Modeling.Models.Abstractions.Collections.Drawings;
using Modeling.Models.Abstractions.SegmentDiemnsions;
using Modeling.Models.Drawing.Figures.PointGeometries;
using Modeling.Models.Miscellaneous;
using System.Collections.Generic;

namespace Modeling.Models.Abstractions.Drawing.Figure
{
    public interface IFigure : IEnumerable<IPointGeometry>, IBounds
    {
        IPointGeometryCollection Segments { get; }

        void CalculatePropertiesFromSegments();
        void CalculateDefaultPropertiesFromSegments();

        ISegmentDimension GetActualSegmentsDimensions();
        void InitializeSegmentDimensions(float pixelsPerCentimeter);
        void ChangeSegmentDimension(SegmentDimensionParameter dimension, float valuePixels);
        void AddSegments(IEnumerable<IPointGeometry> segments, float pixelsPerCentimeter);
        void AddSegment(IPointGeometry segment, float pixelsPerCentimeter);
        void Clear();

        IPointGeometry GetFirstMatchingSegment(PointSingle point, float desiredPointDistance = Constants.DISTANCE_BETWEEN_SEGMENT_POINT_AND_USER_POINT_PIXELS);
    }
}
