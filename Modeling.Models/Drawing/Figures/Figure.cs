using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Miscellaneous;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Models.Drawing.Figures
{
    public sealed class Figure : IFigure
    {
        public LinkedList<IPointGeometry> Segments { get; private set; } = [];

        public void AddSegment(IPointGeometry segment) => Segments.AddLast(segment);

        public void AddSegments(IEnumerable<IPointGeometry> segments)
        {
            foreach (var segment in segments)
            {
                AddSegment(segment);
            }
        }

        public IEnumerator<IPointGeometry> GetEnumerator()
        {
            foreach (var segment in Segments)
            {
                yield return segment;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IPointGeometry GetFirstMatchingSegment(PointSingle point, float desiredPointDistance = Constants.DISTANCE_BETWEEN_SEGMENT_POINT_AND_USER_POINT_PIXELS)
        {
            var result = default(IPointGeometry);

            if (desiredPointDistance <= 0f)
            {
                result = FindMatchingPoint(point);
            }
            else
            {
                result = GetFirstMatchingSegmentCore(point, desiredPointDistance);
            }

            return result;
        }

        IPointGeometry GetFirstMatchingSegmentCore(PointSingle point, float desiredPointDistance)
        {
            var result = default(IPointGeometry);
            var canBreak = false;

            foreach (var segment in Segments)
            {
                foreach (var segmentPoint in segment.Points)
                {
                    if (segmentPoint.CalculateDistance(point) <= desiredPointDistance)
                    {
                        canBreak = true;
                        result = segment;
                        break;
                    }
                }

                if (canBreak)
                {
                    break;
                }
            }

            return result;
        }

        IPointGeometry FindMatchingPoint(PointSingle point)
        {
            var result = default(IPointGeometry);

            foreach (var segment in Segments)
            {
                if (segment.Points.Contains(point))
                {
                    result = segment;
                    break;
                }
            }

            return result;
        }

        public void Clear() => Segments.Clear();
    }
}
