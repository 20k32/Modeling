using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Miscellaneous;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modeling.Models.Drawing.Figures
{
    public sealed class Figure : IFigure
    {
        PointSingle _centerPoint;
        RectangleSingle _bounds;

        public LinkedList<IPointGeometry> Segments { get; private set; } = [];

        public PointSingle CenterPoint => _centerPoint;
        public RectangleSingle Bounds => _bounds;

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

        public void SetPropertiesFromSegments()
        {
            var firstPoint = Segments.First().Points.First();

            var left = float.MaxValue;
            var top = float.MaxValue;
            var width = float.MinValue;
            var height = float.MinValue;

            foreach (var segment in Segments)
            {
                foreach (var point in segment.Points)
                {
                    left = MathF.Min(left, point.X);
                    top = MathF.Min(top, point.Y);
                    width = MathF.Max(width, point.X);
                    height = MathF.Max(height, point.Y);
                }
            }

            _centerPoint = new PointSingle(
                x: width / 2,
                y: height / 2);

            _bounds = new RectangleSingle(top, left, width, height);
        }
    }
}
