using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.Drawing;
using Modeling.Core.Enums;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Models.Abstractions.Collections.Drawings;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Abstractions.SegmentDimensions;
using Modeling.Models.Drawing.Figures.PointGeometries;
using Modeling.Models.Extensions;
using Modeling.Models.Miscellaneous;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Modeling.Models.Drawing.Figures
{
    public sealed class Figure : IFigure
    {
        PointSingle _defaultCenterPoint;
        PointSingle _centerPoint;
        RectangleSingle _bounds;

        public IPointGeometryCollection Segments { get; private init; }
        public ISegmentDimension DefaultSegmentDimensions { get; private init; }
        public ISegmentDimension SegmentDimensions { get; private init; }


        public PointSingle DefaultCenterPoint => _defaultCenterPoint;
        public PointSingle CenterPoint => _centerPoint;
        public RectangleSingle Bounds => _bounds;

        public Figure()
        {
            Segments = Ioc.Default.GetRequiredService<IPointGeometryCollection>();
            DefaultSegmentDimensions = Ioc.Default.GetRequiredService<ISegmentDimension>();
            SegmentDimensions = Ioc.Default.GetRequiredService<ISegmentDimension>();
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

        public void InitializeSegmentDimensions(float pixelsPerCentimeter)
        {
            var pixelsPerMillimeter = pixelsPerCentimeter / 10f;

            var halfCircleDiameter =
                FigureRelatedConstants.HALF_CIRCLES_DIAMETER_MILLIMETERS
                * pixelsPerMillimeter;

            var innerSmallCircleDiameter =
                FigureRelatedConstants.INNER_CIRCLES_DIAMETER_MILLIMETERS
                * pixelsPerMillimeter;

            var verticalDistanceBetweenHalfCircles =
                FigureRelatedConstants.LARGE_CIRCLE_DIAMETER_MILLIMETERS
                * pixelsPerMillimeter;

            var rectangleWidth =
                FigureRelatedConstants.LARGE_RECTANGLE_WIDTH_MILLIMETERS
                * pixelsPerMillimeter;

            var smallSquareSize =
                FigureRelatedConstants.SMALL_SQUARES_DIMENSION_SIZE_MILLIMETERS
                * pixelsPerMillimeter;

            var smallCircleDiameter =
                FigureRelatedConstants.SMALL_CIRCLE_DIAMETER_MILLIMETERS
                * pixelsPerMillimeter;

            var smallLinesDimensionSize =
                FigureRelatedConstants.DISTANCE_BETWEEN_HALF_CIRCLES_AND_LARGE_RECTANGLE
                * pixelsPerMillimeter;

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopLeftHorizontalSmallLineFirstLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomRightHorizontalSmallLineSecondLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopLeftHorizontalSmallLineSecondLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopRightHorizontalSmallLineFirstLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopRightHorizontalSmallLineSecondLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomLeftHorizontalSmallLineFirstLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomLeftHorizontalSmallLineSecondLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomRightHorizontalSmallLineFirstLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopLeftVerticalSmallLineLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.CenterLeftVerticalLargeLineLength,
                verticalDistanceBetweenHalfCircles);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomLeftVerticalSmallLineLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopRightVerticalSmallLineLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.CenterRightVerticalLargeLineLength,
                verticalDistanceBetweenHalfCircles);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomRightVerticalSmallLineLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopLeftHalfCircleDiameter,
                halfCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomLeftHalfCircleDiameter,
                halfCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopRightHalfCircleDiameter,
                halfCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomRightHalfCircleDiameter,
                halfCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopLeftSmallCircleDiameter,
                innerSmallCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopRightSmallCircleDiameter,
                innerSmallCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomLeftSmallCircleDiameter,
                innerSmallCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomRightSmallCircleDiameter,
                innerSmallCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.CenterSmallCircleDiameter,
                smallCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.CenterLargeCircleDiameter,
                verticalDistanceBetweenHalfCircles);

            ChangeSegmentDimension(
                SegmentDimensionParameter.CenterLeftTopHorizontalLineLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.CenterLeftBottomHorizontalLineLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.CenterLeftVerticalLineLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.CenterRightTopHorizontalLineLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.CenterRightBottomHorizontalLineLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.CenterRightVerticalLineLength,
                smallLinesDimensionSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopLargeVerticalLineLength,
                rectangleWidth);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomLargeVerticalLineLength,
                rectangleWidth);
        }

        public void ChangeSegmentDimension(SegmentDimensionParameter dimension, float valuePixels)
        {
            var segmentDimension = SegmentDimensions;

            switch (dimension)
            {
                case SegmentDimensionParameter.TopLeftHorizontalSmallLineFirstLength:
                    segmentDimension.SetTopLeftHorizontalSmallLineFirstLength(valuePixels);
                    break;

                case SegmentDimensionParameter.TopLeftHorizontalSmallLineSecondLength:
                    segmentDimension.SetTopLeftHorizontalSmallLineSecondLength(valuePixels);
                    break;

                case SegmentDimensionParameter.TopRightHorizontalSmallLineFirstLength:
                    segmentDimension.SetTopRightHorizontalSmallLineFirstLength(valuePixels);
                    break;

                case SegmentDimensionParameter.TopRightHorizontalSmallLineSecondLength:
                    segmentDimension.SetTopRightHorizontalSmallLineSecondLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomLeftHorizontalSmallLineFirstLength:
                    segmentDimension.SetBottomLeftHorizontalSmallLineFirstLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomLeftHorizontalSmallLineSecondLength:
                    segmentDimension.SetBottomLeftHorizontalSmallLineSecondLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomRightHorizontalSmallLineFirstLength:
                    segmentDimension.SetBottomRightHorizontalSmallLineFirstLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomRightHorizontalSmallLineSecondLength:
                    segmentDimension.SetBottomRightHorizontalSmallLineSecondLength(valuePixels);
                    break;

                case SegmentDimensionParameter.TopLeftVerticalSmallLineLength:
                    segmentDimension.SetTopLeftVerticalSmallLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.CenterLeftVerticalLargeLineLength:
                    segmentDimension.SetCenterLeftVerticalLargeLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomLeftVerticalSmallLineLength:
                    segmentDimension.SetBottomLeftVerticalSmallLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.TopRightVerticalSmallLineLength:
                    segmentDimension.SetTopRightVerticalSmallLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.CenterRightVerticalLargeLineLength:
                    segmentDimension.SetCenterRightVerticalLargeLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomRightVerticalSmallLineLength:
                    segmentDimension.SetBottomRightVerticalSmallLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.TopLeftHalfCircleDiameter:
                    segmentDimension.SetTopLeftHalfCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomLeftHalfCircleDiameter:
                    segmentDimension.SetBottomLeftHalfCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.TopRightHalfCircleDiameter:
                    segmentDimension.SetTopRightHalfCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomRightHalfCircleDiameter:
                    segmentDimension.SetBottomRightHalfCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.TopLeftSmallCircleDiameter:
                    segmentDimension.SetTopLeftSmallCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomLeftSmallCircleDiameter:
                    segmentDimension.SetBottomLeftSmallCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.TopRightSmallCircleDiameter:
                    segmentDimension.SetTopRightSmallCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomRightSmallCircleDiameter:
                    segmentDimension.SetBottomRightSmallCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.CenterSmallCircleDiameter:
                    segmentDimension.SetCenterSmallCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.CenterLargeCircleDiameter:
                    segmentDimension.SetCenterLargeCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.CenterLeftTopHorizontalLineLength:
                    segmentDimension.SetCenterLeftTopHorizontalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.CenterLeftBottomHorizontalLineLength:
                    segmentDimension.SetCenterLeftBottomHorizontalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.CenterLeftVerticalLineLength:
                    segmentDimension.SetCenterLeftVerticalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.CenterRightTopHorizontalLineLength:
                    segmentDimension.SetCenterRightTopHorizontalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.CenterRightBottomHorizontalLineLength:
                    segmentDimension.SetCenterRightBottomHorizontalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.CenterRightVerticalLineLength:
                    segmentDimension.SetCenterRightVerticalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.TopLargeVerticalLineLength:
                    segmentDimension.SetTopLargeVerticalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomLargeVerticalLineLength:
                    segmentDimension.SetBottomLargeVerticalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.None:
                default:
                    break;
            }
        }

        public ISegmentDimension GetActualSegmentsDimensions() => SegmentDimensions;
    }
}
