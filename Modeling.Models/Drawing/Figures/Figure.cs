using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.Drawing;
using Modeling.Core.Enums;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Models.Abstractions.Collections.Drawings;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Abstractions.SegmentDiemnsions;
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

            var innerCircleDiameter =
                FigureRelatedConstants.INNER_HALF_CIRCLES_DIAMETER_MILLIMETERS
                * pixelsPerMillimeter;

            var halfCircleRadius =
                halfCircleDiameter / 2f;

            var verticalDistance =
                FigureRelatedConstants.VERTICAL_DISTANCE_BETWEEN_HALF_CIRCLES_MILLIMETERS
                * pixelsPerMillimeter;

            var rectangleWidth =
                FigureRelatedConstants.LARGE_RECTANGLE_WIDTH_MILLIMETERS
                * pixelsPerMillimeter;

            var smallSquareSize =
                FigureRelatedConstants.SMALL_SQUARES_DIMENSION_SIZE_MILLIMETERS
                * pixelsPerMillimeter;

            var largeCircleDiameter =
                FigureRelatedConstants.LARGE_CIRCLE_DIAMETER_MILLIMETERS
                * pixelsPerMillimeter;

            var smallCircleDiameter =
                FigureRelatedConstants.SMALL_CIRCLE_DIAMETER_MILLIMETERS
                * pixelsPerMillimeter;

            var distanceToRectangle =
                FigureRelatedConstants.DISTANCE_BETWEEN_HALF_CIRCLES_AND_LARGE_RECTANGLE
                * pixelsPerMillimeter;


            // Circles

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopLeftHalfCircleDiameter,
                halfCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopLeftInnerCircleDiameter,
                innerCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomLeftInnerCircleDiameter,
                innerCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomLeftHalfCircleDiameter,
                halfCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.LargeCircleDiameter,
                largeCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.SmallCircleDiameter,
                smallCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopRightInnerCircleDiameter,
                innerCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopRightHalfCircleDiameter,
                halfCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomRightHalfCircleDiameter,
                halfCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomRightInnerCircleDiameter,
                innerCircleDiameter);


            // Left side

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopHorizontalLineLength,
                rectangleWidth);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomHorizontalLineLength,
                rectangleWidth);

            ChangeSegmentDimension(
                SegmentDimensionParameter.LeftUpperVerticalLineLength,
                (verticalDistance - halfCircleDiameter) / 2f);

            ChangeSegmentDimension(
                SegmentDimensionParameter.LeftMiddleVerticalLineLength,
                verticalDistance - halfCircleDiameter);

            ChangeSegmentDimension(
                SegmentDimensionParameter.LeftLowerVerticalLineLength,
                (verticalDistance - halfCircleDiameter) / 2f);


            // Small squares

            ChangeSegmentDimension(
                SegmentDimensionParameter.LeftSquareVerticalLineLength,
                smallSquareSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.LeftSquareHorizontalLineLength,
                smallSquareSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.RightSquareVerticalLineLength,
                smallSquareSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.RightSquareHorizontalLineLength,
                smallSquareSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomSquareHorizontalLineLength,
                smallSquareSize);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomLeftSquareHorizontalLineLength,
                smallSquareSize);


            // Top-right connection

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopRightConnectionLineLength,
                distanceToRectangle);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopRightLowerConnectionLineLength,
                distanceToRectangle);

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopRightVerticalLineLength,
                halfCircleDiameter);


            // Bottom-right connection

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomRightConnectionLineLength,
                distanceToRectangle);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomRightVerticalLineLength,
                halfCircleDiameter);


            // Circle connection

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopLeftCircleConnectionLineLength,
                distanceToRectangle);

            ChangeSegmentDimension(
                SegmentDimensionParameter.RightMiddleVerticalLineLength,
                verticalDistance);


            // Half-circle top lines

            ChangeSegmentDimension(
                SegmentDimensionParameter.TopLeftHalfCircleTopLineLength,
                distanceToRectangle);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomLeftHalfCircleTopLineLength,
                distanceToRectangle);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomTopLeftHalfCircleTopLineLength,
                distanceToRectangle);

            ChangeSegmentDimension(
                SegmentDimensionParameter.BottomBottomLeftHalfCircleTopLineLength,
                distanceToRectangle);
        }

        public void ChangeSegmentDimension(SegmentDimensionParameter dimension, float valuePixels)
        {
            var segmentDimension = SegmentDimensions;

            switch (dimension)
            {
                case SegmentDimensionParameter.TopLeftHalfCircleDiameter:
                    segmentDimension.SetTopLeftHalfCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.TopLeftInnerCircleDiameter:
                    segmentDimension.SetTopLeftInnerCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomLeftInnerCircleDiameter:
                    segmentDimension.SetBottomLeftInnerCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomLeftHalfCircleDiameter:
                    segmentDimension.SetBottomLeftHalfCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.LargeCircleDiameter:
                    segmentDimension.SetLargeCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.SmallCircleDiameter:
                    segmentDimension.SetSmallCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.TopRightInnerCircleDiameter:
                    segmentDimension.SetTopRightInnerCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.TopRightHalfCircleDiameter:
                    segmentDimension.SetTopRightHalfCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomRightHalfCircleDiameter:
                    segmentDimension.SetBottomRightHalfCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomRightInnerCircleDiameter:
                    segmentDimension.SetBottomRightInnerCircleDiameter(valuePixels);
                    break;

                case SegmentDimensionParameter.TopHorizontalLineLength:
                    segmentDimension.SetTopHorizontalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomHorizontalLineLength:
                    segmentDimension.SetBottomHorizontalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.LeftUpperVerticalLineLength:
                    segmentDimension.SetLeftUpperVerticalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.LeftMiddleVerticalLineLength:
                    segmentDimension.SetLeftMiddleVerticalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.LeftLowerVerticalLineLength:
                    segmentDimension.SetLeftLowerVerticalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.LeftSquareVerticalLineLength:
                    segmentDimension.SetLeftSquareVerticalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.LeftSquareHorizontalLineLength:
                    segmentDimension.SetLeftSquareHorizontalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.RightSquareVerticalLineLength:
                    segmentDimension.SetRightSquareVerticalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.RightSquareHorizontalLineLength:
                    segmentDimension.SetRightSquareHorizontalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomSquareHorizontalLineLength:
                    segmentDimension.SetBottomSquareHorizontalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomLeftSquareHorizontalLineLength:
                    segmentDimension.SetBottomLeftSquareHorizontalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.TopRightConnectionLineLength:
                    segmentDimension.SetTopRightConnectionLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.TopRightLowerConnectionLineLength:
                    segmentDimension.SetTopRightLowerConnectionLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.TopRightVerticalLineLength:
                    segmentDimension.SetTopRightVerticalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomRightConnectionLineLength:
                    segmentDimension.SetBottomRightConnectionLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.TopLeftCircleConnectionLineLength:
                    segmentDimension.SetTopLeftCircleConnectionLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.RightMiddleVerticalLineLength:
                    segmentDimension.SetRightMiddleVerticalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomRightVerticalLineLength:
                    segmentDimension.SetBottomRightVerticalLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.TopLeftHalfCircleTopLineLength:
                    segmentDimension.SetTopLeftHalfCircleTopLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomLeftHalfCircleTopLineLength:
                    segmentDimension.SetBottomLeftHalfCircleTopLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomTopLeftHalfCircleTopLineLength:
                    segmentDimension.SetBottomTopLeftHalfCircleTopLineLength(valuePixels);
                    break;

                case SegmentDimensionParameter.BottomBottomLeftHalfCircleTopLineLength:
                    segmentDimension.SetBottomBottomLeftHalfCircleTopLineLength(valuePixels);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(dimension), dimension, null);
            }
        }

        public ISegmentDimension GetActualSegmentsDimensions() => SegmentDimensions;
    }
}
