using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Miscellaneous;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.Figures
{
    public sealed class Figure : IFigure
    {
        PointSingle _centerPoint;
        RectangleSingle _bounds;

        public LinkedList<IPointGeometry> Segments { get; private set; } = [];

        public PointSingle CenterPoint => _centerPoint;
        public RectangleSingle Bounds => _bounds;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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

        public void Clear() => Segments.Clear();

        public void SetBounds()
        {
            var left = float.MaxValue;
            var top = float.MaxValue;
            var width = float.MinValue;
            var height = float.MinValue;

            foreach (var segment in Segments)
            {
                segment.SetBounds();

                left = MathF.Min(left, segment.Bounds.Left);
                top = MathF.Min(top, segment.Bounds.Top);
                width = MathF.Max(width, segment.Bounds.Right);
                height = MathF.Max(height, segment.Bounds.Bottom);
            }

            _bounds = new RectangleSingle(top, left, width, height);
        }

        public void SetPropertiesFromSegments()
        {
            SetBounds();

            _centerPoint = new PointSingle(
                x: _bounds.Right / 2,
                y: _bounds.Bottom / 2);
        }
    }
}
