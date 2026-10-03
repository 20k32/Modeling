using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Models.Abstractions.Collections.Drawings;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Miscellaneous;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.Figures
{
    public sealed class Figure : IFigure
    {
        PointSingle _defaultCenterPoint;
        PointSingle _centerPoint;
        RectangleSingle _bounds;

        public IPointGeometryCollection Segments { get; private init; }

        public PointSingle DefaultCenterPoint => _defaultCenterPoint;
        public PointSingle CenterPoint => _centerPoint;
        public RectangleSingle Bounds => _bounds;

        public Figure()
        {
            Segments = Ioc.Default.GetRequiredService<IPointGeometryCollection>();
        }

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
                if (segment.ContainsPoint(point))
                {
                    result = segment;
                    break;
                }
            }

            return result;
        }

        public void AddSegment(IPointGeometry segment, float pixelsPerCentimeter)
        {
            if (!Segments.Contains(segment))
            {
                foreach (var existingSegment in Segments)
                {
                    if (existingSegment.Bounds.IntersectsBounds(segment.Bounds, pixelsPerCentimeter))
                    {
                        segment.AddAdjacentGeometry(existingSegment);
                    }
                }

                Segments.Add(segment);
            }
        }

        public void AddSegments(IEnumerable<IPointGeometry> segments, float pixelsPerCentimeter)
        {
            foreach (var segment in segments)
            {
                AddSegment(segment, pixelsPerCentimeter);
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

        public void CalculateBounds()
        {
            var left = float.MaxValue;
            var top = float.MaxValue;
            var right = float.MinValue;
            var bottom = float.MinValue;

            foreach (var segment in Segments)
            {
                left = MathF.Min(left, segment.Bounds.Left);
                top = MathF.Min(top, segment.Bounds.Top);
                right = MathF.Max(right, segment.Bounds.Right);
                bottom = MathF.Max(bottom, segment.Bounds.Bottom);
            }

            _bounds = new RectangleSingle(top, left, right, bottom);
        }

        public void CalculatePropertiesFromSegments()
        {
            CalculateBounds();
            CalculateCenterPoint();
        }

        public void CalculateDefaultPropertiesFromSegments()
        {
            _defaultCenterPoint = CenterPoint;

            foreach (var segment in Segments)
            {
                segment.SetDefaultProperties();
            }
        }

        public void CalculateCenterPoint() => _centerPoint = new PointSingle(
                x: _bounds.Width / 2,
                y: _bounds.Height / 2);

        public void UpdateAdjacentGeometriesBounds()
        {

        }
    }
}
