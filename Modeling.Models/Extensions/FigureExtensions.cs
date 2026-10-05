using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Abstractions.SegmentDimensions;
using Modeling.Models.Drawing.Figures.PointGeometries;
using Modeling.Models.Miscellaneous;
using System;
using System.Collections.Generic;

namespace Modeling.Models.Extensions
{
    public static class FigureExtensions
    {
        public static IPointGeometry FindNearestSegment(this IFigure figure, PointSingle point, float pixelsPerCentimeter)
        {
            var nearestSegment = default(IPointGeometry);

            foreach (var segment in figure)
            {
                if (segment.Bounds.Contains(point, pixelsPerCentimeter))
                {
                    nearestSegment = segment;
                }
            }

            return nearestSegment;
        }

        public static IEnumerable<PointSingle> CreateGrid(SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            for (var y = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                y <= canvasSize.Height; y += pixelsPerCentimeter)
            {
                for (var x = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                    x <= canvasSize.Width + pixelsPerCentimeter; x += pixelsPerCentimeter)
                {
                    yield return new PointSingle(x, y);
                }

                if (y < canvasSize.Height)
                {
                    yield return DrawingConstants.BREAK_POINT;
                }
            }

            for (var x = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                x <= canvasSize.Width; x += pixelsPerCentimeter)
            {
                for (var y = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                    y <= canvasSize.Height + pixelsPerCentimeter; y += pixelsPerCentimeter)
                {
                    yield return new PointSingle(x, y);
                }

                if (x < canvasSize.Width)
                {
                    yield return DrawingConstants.BREAK_POINT;
                }
            }
        }

        public static IEnumerable<PointSingle> CreateAxisLine(float dimensionSize, float pixelsPerCentimeter, float centerX, float centerY, bool isVertical)
        {
            if (isVertical)
            {
                yield return new PointSingle(centerX, centerY - pixelsPerCentimeter);
                yield return new PointSingle(centerX, dimensionSize + pixelsPerCentimeter);
            }
            else
            {
                yield return new PointSingle(centerX, centerY);
                yield return new PointSingle(dimensionSize + pixelsPerCentimeter, centerY);
            }

            yield return DrawingConstants.BREAK_POINT;

            if (isVertical)
            {
                yield return new PointSingle(centerX, centerY - pixelsPerCentimeter);
                yield return new PointSingle(centerX, 0);
            }
            else
            {
                yield return new PointSingle(centerX, centerY);
                yield return new PointSingle(0, centerY);
            }

            yield return DrawingConstants.BREAK_POINT;
        }

        public static IEnumerable<PointSingle> CreateArrowHead(PointSingle endPoint, bool isVertical, float arrowHeadSize, bool pointingLeft = false, bool pointingUp = false)
        {
            yield return DrawingConstants.BREAK_POINT;

            if (isVertical)
            {
                var tipX = endPoint.X;
                var tipY = endPoint.Y;

                yield return new PointSingle(tipX, tipY);

                if (pointingUp)
                {
                    yield return new PointSingle(tipX - arrowHeadSize / 2, tipY + arrowHeadSize);
                    yield return new PointSingle(tipX, tipY);
                    yield return new PointSingle(tipX + arrowHeadSize / 2, tipY + arrowHeadSize);
                }
                else
                {
                    yield return new PointSingle(tipX - arrowHeadSize / 2, tipY - arrowHeadSize);
                    yield return new PointSingle(tipX, tipY);
                    yield return new PointSingle(tipX + arrowHeadSize / 2, tipY - arrowHeadSize);
                }

                yield return new PointSingle(tipX, tipY);
            }
            else
            {
                var tipX = endPoint.X;
                var tipY = endPoint.Y;

                yield return new PointSingle(tipX, tipY);

                if (pointingLeft)
                {
                    yield return new PointSingle(tipX + arrowHeadSize, tipY - arrowHeadSize / 2);
                    yield return new PointSingle(tipX, tipY);
                    yield return new PointSingle(tipX + arrowHeadSize, tipY + arrowHeadSize / 2);
                }
                else
                {
                    yield return new PointSingle(tipX - arrowHeadSize, tipY - arrowHeadSize / 2);
                    yield return new PointSingle(tipX, tipY);
                    yield return new PointSingle(tipX - arrowHeadSize, tipY + arrowHeadSize / 2);
                }
                yield return new PointSingle(tipX, tipY);
            }

            yield return DrawingConstants.BREAK_POINT;
        }

        public static IEnumerable<PointSingle> CreateAxisMarks(float dimensionSize, float pixelsPerCentimeter, float tickStart, float tickEnd, float centerX, float centerY, bool isVertical)
        {
            if (isVertical)
            {
                for (var axisPosition = centerY; axisPosition < dimensionSize; axisPosition += pixelsPerCentimeter)
                {
                    for (var tickOffset = tickStart; tickOffset <= tickEnd; tickOffset++)
                    {
                        yield return new PointSingle(centerX + tickOffset, axisPosition);
                    }

                    yield return DrawingConstants.BREAK_POINT;
                }
            }
            else
            {
                for (var axisPosition = centerX; axisPosition < dimensionSize; axisPosition += pixelsPerCentimeter)
                {
                    for (var tickOffset = tickStart; tickOffset <= tickEnd; tickOffset++)
                    {
                        yield return new PointSingle(axisPosition, centerY + tickOffset);
                    }

                    yield return DrawingConstants.BREAK_POINT;
                }
            }

            yield return DrawingConstants.BREAK_POINT;

            if (isVertical)
            {
                for (var axisPosition = centerY - pixelsPerCentimeter; axisPosition > 0; axisPosition -= pixelsPerCentimeter)
                {
                    for (var tickOffset = tickStart; tickOffset <= tickEnd; tickOffset++)
                    {
                        yield return new PointSingle(centerX + tickOffset, axisPosition);
                    }

                    yield return DrawingConstants.BREAK_POINT;
                }
            }
            else
            {
                for (var axisPosition = centerX - pixelsPerCentimeter; axisPosition > 0; axisPosition -= pixelsPerCentimeter)
                {
                    for (var tickOffset = tickStart; tickOffset <= tickEnd; tickOffset++)
                    {
                        yield return new PointSingle(axisPosition, centerY + tickOffset);
                    }
                    yield return DrawingConstants.BREAK_POINT;
                }
            }

            yield return DrawingConstants.BREAK_POINT;
        }

        /* public static IEnumerable<IPointGeometry> CreateCustomShape(PointSingle startDrawingPoint, ISegmentDimension dimension)
         {
             var leftHalfCirclesStartAngleDegrees = 90f;
             var leftHalfCirclesEndAngleDegrees = 270f;

             var rightHalfCirclesStartAngleDegrees = -90f;
             var rightHalfCirclesEndAngleDegrees = 90f;


         }*/

        public static IEnumerable<IPointGeometry> CreateCustomShape(PointSingle startDrawingPoint, ISegmentDimension dimension)
        {
            var leftHalfCirclesStartAngleDegrees = 90f;
            var leftHalfCirclesEndAngleDegrees = 270f;

            var rightHalfCirclesStartAngleDegrees = -90f;
            var rightHalfCirclesEndAngleDegrees = 90f;

            var topLargeVerticalLineWidth = dimension.TopLargeVerticalLineLength;
            var bottomLargeVerticalLineWidth = dimension.BottomLargeVerticalLineLength;

            var topLeftHorizontalSmallLineFirstWidth = dimension.TopLeftHorizontalSmallLineFirstLength;
            var topLeftHorizontalSmallLineSecondWidth = dimension.TopLeftHorizontalSmallLineSecondLength;
            var topRightHorizontalSmallLineFirstWidth = dimension.TopRightHorizontalSmallLineFirstLength;
            var topRightHorizontalSmallLineSecondWidth = dimension.TopRightHorizontalSmallLineSecondLength;

            var bottomLeftHorizontalSmallLineFirstWidth = dimension.BottomLeftHorizontalSmallLineFirstLength;
            var bottomLeftHorizontalSmallLineSecondWidth = dimension.BottomLeftHorizontalSmallLineSecondLength;
            var bottomRightHorizontalSmallLineFirstWidth = dimension.BottomRightHorizontalSmallLineFirstLength;
            var bottomRightHorizontalSmallLineSecondWidth = dimension.BottomRightHorizontalSmallLineSecondLength;

            var topLeftVerticalLineHeight = dimension.TopLeftVerticalSmallLineLength;
            var centerLeftLargeVerticalLineHeight = dimension.CenterLeftVerticalLargeLineLength;
            var bottomLeftVerticalSmallLineHeight = dimension.BottomLeftVerticalSmallLineLength;

            var topRightVerticalSmallLineHeight = dimension.TopRightVerticalSmallLineLength;
            var centerRightLargeVerticalLineHeight = dimension.CenterRightVerticalLargeLineLength;
            var bottomRightVerticalSmallLineHeight = dimension.BottomRightVerticalSmallLineLength;

            var topLeftHalfCircleRadius = dimension.TopLeftHalfCircleDiameter / 2;
            var topRightHalfCircleRadius = dimension.TopRightHalfCircleDiameter / 2;
            var bottomLeftHalfCircleRadius = dimension.BottomLeftHalfCircleDiameter / 2;
            var bottomRightHalfCircleRadius = dimension.BottomRightHalfCircleDiameter / 2;

            var smallCenterCircleRadius = dimension.CenterSmallCircleDiameter / 2;
            var largeCenterCircleRadius = dimension.CenterLargeCircleDiameter / 2;

            var topLeftCircleRadius = dimension.TopLeftSmallCircleDiameter / 2;
            var topRightCircleRadius = dimension.TopRightSmallCircleDiameter / 2;
            var bottomLeftCircleRadius = dimension.BottomLeftSmallCircleDiameter / 2;
            var bottomRightCircleDiameter = dimension.BottomRightSmallCircleDiameter / 2;

            var centerLeftHorizontalLineFirstWidth = dimension.CenterLeftTopHorizontalLineLength;
            var centerLeftHorizontalLineSecondWidth = dimension.CenterLeftBottomHorizontalLineLength;
            var centerLeftVerticalLineHeight = dimension.CenterLeftVerticalLineLength;

            var centerRightVerticalLineHeight = dimension.CenterRightVerticalLineLength;
            var centerRightHorizontalLineFirstWidth = dimension.CenterRightTopHorizontalLineLength;
            var centerRightHorizontalLineSecondWidth = dimension.CenterRightBottomHorizontalLineLength;

            var topLeftHalfCircleCenterPoint = startDrawingPoint;

            var topLeftHalfCircle = topLeftHalfCircleCenterPoint.GetCirclePoints(
                topLeftHalfCircleRadius,
                leftHalfCirclesStartAngleDegrees,
                leftHalfCirclesEndAngleDegrees);

            var topLeftInnerCircleCenterPoint = topLeftHalfCircleCenterPoint;

            var topLeftInnerCircle = topLeftInnerCircleCenterPoint.GetCirclePoints(
                topLeftCircleRadius);

            var bottomLeftHalfCircleCenterPointY = topLeftHalfCircleCenterPoint.Y
                + topLeftHalfCircleRadius
                + bottomLeftHalfCircleRadius
                + centerLeftLargeVerticalLineHeight;

            var bottomLeftHalfCircleCenterPoint = new PointSingle(
                x: topLeftHalfCircleCenterPoint.X,
                y: bottomLeftHalfCircleCenterPointY);

            var bottomLeftHalfCircle = bottomLeftHalfCircleCenterPoint.GetCirclePoints(
                bottomLeftHalfCircleRadius,
                leftHalfCirclesStartAngleDegrees,
                leftHalfCirclesEndAngleDegrees);

            var bottomLeftInnerCircleCenterPoint = bottomLeftHalfCircleCenterPoint;

            var bottomLeftInnerCircle = bottomLeftInnerCircleCenterPoint.GetCirclePoints(bottomLeftCircleRadius);

            yield return topLeftHalfCircle.ToCirclePointGeometry(topLeftHalfCircleCenterPoint,
                parameter: SegmentDimensionParameter.TopLeftHalfCircleDiameter);

            yield return topLeftInnerCircle.ToCirclePointGeometry(topLeftInnerCircleCenterPoint,
                parameter: SegmentDimensionParameter.TopLeftSmallCircleDiameter);

            yield return bottomLeftHalfCircle.ToCirclePointGeometry(bottomLeftHalfCircleCenterPoint,
                parameter: SegmentDimensionParameter.BottomLeftHalfCircleDiameter);

            yield return bottomLeftInnerCircle.ToCirclePointGeometry(bottomLeftInnerCircleCenterPoint,
                parameter: SegmentDimensionParameter.BottomLeftSmallCircleDiameter);

            var topTopLeftHorizontalSmallLineFirstPoint = new PointSingle(
                x: topLeftHalfCircleCenterPoint.X,
                y: topLeftHalfCircleCenterPoint.Y - topLeftHalfCircleRadius);

            var topTopLeftHorizontalSmallLineSecondPoint = new PointSingle(
                x: topTopLeftHorizontalSmallLineFirstPoint.X + topLeftHorizontalSmallLineFirstWidth,
                y: topTopLeftHorizontalSmallLineFirstPoint.Y);

            yield return ((ICollection<PointSingle>)[topTopLeftHorizontalSmallLineFirstPoint,
                topTopLeftHorizontalSmallLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.TopLeftHorizontalSmallLineFirstLength);

            var topBottomLeftHorizontalSmallLineFirstPoint = new PointSingle(
                x: topLeftHalfCircleCenterPoint.X,
                y: topLeftHalfCircleCenterPoint.Y + topLeftHalfCircleRadius);

            var topBottomLeftHorizontalSmallLineSecondPoint = new PointSingle(
                x: topBottomLeftHorizontalSmallLineFirstPoint.X + topLeftHorizontalSmallLineSecondWidth,
                y: topBottomLeftHorizontalSmallLineFirstPoint.Y);

            yield return ((ICollection<PointSingle>)[topBottomLeftHorizontalSmallLineFirstPoint,
                topBottomLeftHorizontalSmallLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.TopLeftHorizontalSmallLineSecondLength);

            var topLeftVerticalSmallLineFirstPoint = topTopLeftHorizontalSmallLineSecondPoint;

            var topLeftVerticalSmallLineSecondPoint = new PointSingle(
                x: topLeftVerticalSmallLineFirstPoint.X,
                y: topLeftVerticalSmallLineFirstPoint.Y + topLeftVerticalLineHeight);

            yield return ((ICollection<PointSingle>)[topLeftVerticalSmallLineFirstPoint,
                topLeftVerticalSmallLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.TopLeftVerticalSmallLineLength);

            var topLeftVerticalLargeLineFirstPoint = topLeftVerticalSmallLineSecondPoint;

            var topLeftVerticalLargeLineSecondPoint = new PointSingle(
                x: topLeftVerticalLargeLineFirstPoint.X,
                y: topLeftVerticalLargeLineFirstPoint.Y + centerLeftLargeVerticalLineHeight);

            yield return ((ICollection<PointSingle>)[topLeftVerticalLargeLineFirstPoint,
                topLeftVerticalLargeLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.CenterLeftVerticalLargeLineLength);

            var bottomTopLeftHorizontalSmallLineFirstPoint = new PointSingle(
               x: bottomLeftHalfCircleCenterPoint.X,
               y: bottomLeftHalfCircleCenterPoint.Y - bottomLeftHalfCircleRadius);

            var bottomTopLeftHorizontalSmallLineSecondPoint = new PointSingle(
                x: bottomTopLeftHorizontalSmallLineFirstPoint.X + bottomLeftHorizontalSmallLineFirstWidth,
                y: bottomTopLeftHorizontalSmallLineFirstPoint.Y);

            yield return ((ICollection<PointSingle>)[bottomTopLeftHorizontalSmallLineFirstPoint,
                bottomTopLeftHorizontalSmallLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.BottomLeftHorizontalSmallLineFirstLength);

            var bottomBottomLeftHorizontalSmallLineFirstPoint = new PointSingle(
               x: bottomLeftHalfCircleCenterPoint.X,
               y: bottomLeftHalfCircleCenterPoint.Y + bottomLeftHalfCircleRadius);

            var bottomBottomLeftHorizontalSmallLineSecondPoint = new PointSingle(
                x: bottomBottomLeftHorizontalSmallLineFirstPoint.X + bottomLeftHorizontalSmallLineSecondWidth,
                y: bottomBottomLeftHorizontalSmallLineFirstPoint.Y);

            yield return ((ICollection<PointSingle>)[bottomBottomLeftHorizontalSmallLineFirstPoint,
                 bottomBottomLeftHorizontalSmallLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.BottomLeftHorizontalSmallLineSecondLength);

            var bottomLeftVerticalSmallLineFirstPoint = bottomTopLeftHorizontalSmallLineSecondPoint;

            var bottomLeftVerticalSmallLineSecondPoint = new PointSingle(
                x: bottomLeftVerticalSmallLineFirstPoint.X,
                y: bottomLeftVerticalSmallLineFirstPoint.Y + bottomLeftVerticalSmallLineHeight);

            yield return ((ICollection<PointSingle>)[bottomLeftVerticalSmallLineFirstPoint,
                bottomLeftVerticalSmallLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.BottomLeftVerticalSmallLineLength);

            var topLargeLineFirstPoint = topTopLeftHorizontalSmallLineSecondPoint;

            var topLargeLineSecondPoint = new PointSingle(
                x: topLargeLineFirstPoint.X + topLargeVerticalLineWidth,
                y: topLargeLineFirstPoint.Y);

            yield return ((ICollection<PointSingle>)[topLargeLineFirstPoint,
                topLargeLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.TopLargeVerticalLineLength);

            var bottomLargeLineFirstPoint = bottomBottomLeftHorizontalSmallLineSecondPoint;

            var bottomLargeLineSecondPoint = new PointSingle(
                x: bottomBottomLeftHorizontalSmallLineSecondPoint.X + bottomLargeVerticalLineWidth,
                y: bottomBottomLeftHorizontalSmallLineSecondPoint.Y);

            yield return ((ICollection<PointSingle>)[bottomLargeLineFirstPoint,
                bottomLargeLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.BottomLargeVerticalLineLength);

            var topTopRightVerticalSmallLineFirstPoint = topLargeLineSecondPoint;

            var topTopRightVerticalSmallLineFirstSecondPoint = new PointSingle(
                x: topTopRightVerticalSmallLineFirstPoint.X + topRightHorizontalSmallLineFirstWidth,
                y: topTopRightVerticalSmallLineFirstPoint.Y);

            yield return ((ICollection<PointSingle>)[topTopRightVerticalSmallLineFirstPoint,
                topTopRightVerticalSmallLineFirstSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.TopRightHorizontalSmallLineFirstLength);

            var topBottomRightVerticalSmallLineFirstPoint = new PointSingle(
                x: topLargeLineSecondPoint.X,
                y: topLargeLineSecondPoint.Y + topRightVerticalSmallLineHeight);

            var topBottomRightVerticalSmallLineFirstSecondPoint = new PointSingle(
                x: topBottomRightVerticalSmallLineFirstPoint.X + topRightHorizontalSmallLineSecondWidth,
                y: topBottomRightVerticalSmallLineFirstPoint.Y);

            yield return ((ICollection<PointSingle>)[topBottomRightVerticalSmallLineFirstPoint,
                 topBottomRightVerticalSmallLineFirstSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.TopRightHorizontalSmallLineSecondLength);

            var topRightHalfCircleCenterPoint = new PointSingle(
                x: topTopRightVerticalSmallLineFirstSecondPoint.X,
                y: topTopRightVerticalSmallLineFirstSecondPoint.Y + topRightHalfCircleRadius);

            var topRightHalfCircle = topRightHalfCircleCenterPoint.GetCirclePoints(
                topRightHalfCircleRadius,
                rightHalfCirclesStartAngleDegrees,
                rightHalfCirclesEndAngleDegrees);

            yield return topRightHalfCircle.ToCirclePointGeometry(topRightHalfCircleCenterPoint,
                parameter: SegmentDimensionParameter.TopRightHalfCircleDiameter);

            var topRightInnerCircle = topRightHalfCircleCenterPoint.GetCirclePoints(
                topRightCircleRadius);

            yield return topRightInnerCircle.ToCirclePointGeometry(topRightHalfCircleCenterPoint,
                parameter: SegmentDimensionParameter.TopRightSmallCircleDiameter);

            var topRightVerticalSmallLineFirstPoint = topTopRightVerticalSmallLineFirstPoint;
            var topRightVerticalSmallLineSecondPoint = new PointSingle(
                x: topRightVerticalSmallLineFirstPoint.X,
                y: topRightVerticalSmallLineFirstPoint.Y + topRightVerticalSmallLineHeight);

            yield return ((ICollection<PointSingle>)[topRightVerticalSmallLineFirstPoint,
                topRightVerticalSmallLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.TopRightVerticalSmallLineLength);

            var topRightVerticalLineFirstPoint = topBottomRightVerticalSmallLineFirstPoint;
            var rightVerticalLineSecondPoint = new PointSingle(
                x: topRightVerticalLineFirstPoint.X,
                y: topRightVerticalLineFirstPoint.Y + centerRightLargeVerticalLineHeight);

            yield return ((ICollection<PointSingle>)[topRightVerticalLineFirstPoint,
                rightVerticalLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.CenterRightVerticalLargeLineLength);

            var bottomTopRightHorizontalSmallLineFirstPoint = rightVerticalLineSecondPoint;
            var bottomTopRightHorizontalSmallLineSecondPoint = new PointSingle(
                x: bottomTopRightHorizontalSmallLineFirstPoint.X + bottomRightHorizontalSmallLineFirstWidth,
                y: bottomTopRightHorizontalSmallLineFirstPoint.Y);

            yield return ((ICollection<PointSingle>)[bottomTopRightHorizontalSmallLineFirstPoint,
                bottomTopRightHorizontalSmallLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.BottomRightHorizontalSmallLineFirstLength);

            var bottomBottomRightHorizontalSmallLineFirstPoint = new PointSingle(
                x: rightVerticalLineSecondPoint.X,
                y: rightVerticalLineSecondPoint.Y + bottomRightVerticalSmallLineHeight);

            var bottomBottomRightHorizontalSmallLineSecondPoint = new PointSingle(
                x: bottomBottomRightHorizontalSmallLineFirstPoint.X + bottomRightHorizontalSmallLineSecondWidth,
                y: bottomBottomRightHorizontalSmallLineFirstPoint.Y);

            yield return ((ICollection<PointSingle>)[bottomBottomRightHorizontalSmallLineFirstPoint,
                bottomBottomRightHorizontalSmallLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.BottomRightHorizontalSmallLineSecondLength);

            var bottomRightVerticalSmallLineFirstPoint = rightVerticalLineSecondPoint;

            var bottomRightVerticalSmallLineSecondPoint = new PointSingle(
                x: bottomRightVerticalSmallLineFirstPoint.X,
                y: bottomRightVerticalSmallLineFirstPoint.Y + bottomRightVerticalSmallLineHeight);

            yield return ((ICollection<PointSingle>)[bottomRightVerticalSmallLineFirstPoint,
                bottomRightVerticalSmallLineSecondPoint])
                .ToLinePointGeometry(SegmentDimensionParameter.BottomRightVerticalSmallLineLength);

            var bottomRightHalfCircleCenterPoint = new PointSingle(
                x: bottomTopRightHorizontalSmallLineSecondPoint.X,
                y: bottomTopRightHorizontalSmallLineSecondPoint.Y + bottomRightHalfCircleRadius);

            var bottomRightHalfCircle = bottomRightHalfCircleCenterPoint.GetCirclePoints(
                bottomRightHalfCircleRadius,
                rightHalfCirclesStartAngleDegrees,
                rightHalfCirclesEndAngleDegrees);

            yield return bottomRightHalfCircle.ToCirclePointGeometry(bottomRightHalfCircleCenterPoint,
                parameter: SegmentDimensionParameter.BottomRightHalfCircleDiameter);

            var bottomRightInnerCircle = bottomRightHalfCircleCenterPoint.GetCirclePoints(
                bottomRightCircleDiameter);

            yield return bottomRightInnerCircle.ToCirclePointGeometry(bottomRightHalfCircleCenterPoint);

            var verticalLeftLargeLineHalfDistance = centerLeftLargeVerticalLineHeight / 2;
            var topVerticalLeftSmallLineDistance = topLeftVerticalLineHeight;

            var overallTopVerticalHalfDistance = verticalLeftLargeLineHalfDistance + topVerticalLeftSmallLineDistance;
            var horizontalOffset = topLargeLineFirstPoint.X + topLargeVerticalLineWidth / 2;
            var verticalOffset = topLargeLineFirstPoint.Y + overallTopVerticalHalfDistance;

            var largeCenterCircleCenterPoint = new PointSingle(
                x: horizontalOffset,
                y: verticalOffset);

            var largeCircle = largeCenterCircleCenterPoint.GetCirclePoints(largeCenterCircleRadius);

            yield return largeCircle.ToCirclePointGeometry(largeCenterCircleCenterPoint,
                parameter: SegmentDimensionParameter.CenterLargeCircleDiameter);

            var smallCircle = largeCenterCircleCenterPoint.GetCirclePoints(smallCenterCircleRadius);

            yield return smallCircle.ToCirclePointGeometry(largeCenterCircleCenterPoint,
                parameter: SegmentDimensionParameter.CenterSmallCircleDiameter);

            var horizontalDistanceBetweenLeftSmallVerticalLineAndLargeCircle =
    centerLeftHorizontalLineFirstWidth;

            var leftSmallVerticalCenterLineStartPoint = new PointSingle(
                x: horizontalOffset
                   - largeCenterCircleRadius
                   - horizontalDistanceBetweenLeftSmallVerticalLineAndLargeCircle,
                y: verticalOffset - (centerLeftVerticalLineHeight / 2));

            var leftSmallVerticalCenterLineSecondPoint = new PointSingle(
                x: leftSmallVerticalCenterLineStartPoint.X,
                y: leftSmallVerticalCenterLineStartPoint.Y
                   + centerLeftVerticalLineHeight);

            yield return ((ICollection<PointSingle>)[
                    leftSmallVerticalCenterLineStartPoint,
        leftSmallVerticalCenterLineSecondPoint])
                .ToLinePointGeometry(
                    SegmentDimensionParameter.CenterLeftVerticalLineLength);

            var topSquareDistanceFromCircleCenter =
                leftSmallVerticalCenterLineStartPoint.Y - verticalOffset;

            var bottomSquareDistanceFromCircleCenter =
                leftSmallVerticalCenterLineSecondPoint.Y - verticalOffset;

            var topCircleHorizontalOffset = MathF.Sqrt(
                MathF.Max(
                    0f,
                    largeCenterCircleRadius * largeCenterCircleRadius
                    - topSquareDistanceFromCircleCenter * topSquareDistanceFromCircleCenter));

            var bottomCircleHorizontalOffset = MathF.Sqrt(
                MathF.Max(
                    0f,
                    largeCenterCircleRadius * largeCenterCircleRadius
                    - bottomSquareDistanceFromCircleCenter * bottomSquareDistanceFromCircleCenter));

            var leftSmallCenterHorizonalFirstLineFistPoint =
                leftSmallVerticalCenterLineStartPoint;

            var leftTopCircleIntersectionPoint = new PointSingle(
                x: horizontalOffset - topCircleHorizontalOffset,
                y: leftSmallCenterHorizonalFirstLineFistPoint.Y);

            yield return ((ICollection<PointSingle>)[
                    leftSmallCenterHorizonalFirstLineFistPoint,
        leftTopCircleIntersectionPoint])
                .ToLinePointGeometry(
                    SegmentDimensionParameter.CenterLeftTopHorizontalLineLength);

            var leftSmallCenterHorizonalSecondLineFistPoint =
                leftSmallVerticalCenterLineSecondPoint;

            var leftBottomCircleIntersectionPoint = new PointSingle(
                x: horizontalOffset - bottomCircleHorizontalOffset,
                y: leftSmallCenterHorizonalSecondLineFistPoint.Y);

            yield return ((ICollection<PointSingle>)[
                    leftSmallCenterHorizonalSecondLineFistPoint,
        leftBottomCircleIntersectionPoint])
                .ToLinePointGeometry(
                    SegmentDimensionParameter.CenterLeftBottomHorizontalLineLength);

            var rightSmallVerticalCenterLineStartPoint = new PointSingle(
                x: horizontalOffset
                   + largeCenterCircleRadius
                   + horizontalDistanceBetweenLeftSmallVerticalLineAndLargeCircle,
                y: verticalOffset - (centerRightVerticalLineHeight / 2));

            var rightSmallVerticalCenterLineEndPoint = new PointSingle(
                x: rightSmallVerticalCenterLineStartPoint.X,
                y: rightSmallVerticalCenterLineStartPoint.Y
                   + centerRightVerticalLineHeight);

            yield return ((ICollection<PointSingle>)[
                    rightSmallVerticalCenterLineStartPoint,
        rightSmallVerticalCenterLineEndPoint])
                .ToLinePointGeometry(
                    SegmentDimensionParameter.CenterRightVerticalLineLength);

            var rightSmallCenterHorizontalLineFirstPoint =
                rightSmallVerticalCenterLineStartPoint;

            var rightTopCircleIntersectionPoint = new PointSingle(
                x: horizontalOffset + topCircleHorizontalOffset,
                y: rightSmallCenterHorizontalLineFirstPoint.Y);

            yield return ((ICollection<PointSingle>)[
                    rightSmallCenterHorizontalLineFirstPoint,
        rightTopCircleIntersectionPoint])
                .ToLinePointGeometry(
                    SegmentDimensionParameter.CenterRightTopHorizontalLineLength);

            var rightSmallCenterHorizontalSecondLineFirstPoint =
                rightSmallVerticalCenterLineEndPoint;

            var rightBottomCircleIntersectionPoint = new PointSingle(
                x: horizontalOffset + bottomCircleHorizontalOffset,
                y: rightSmallCenterHorizontalSecondLineFirstPoint.Y);

            yield return ((ICollection<PointSingle>)[
                    rightSmallCenterHorizontalSecondLineFirstPoint,
        rightBottomCircleIntersectionPoint])
                .ToLinePointGeometry(
                    SegmentDimensionParameter.CenterRightBottomHorizontalLineLength);
        }
    }
}
