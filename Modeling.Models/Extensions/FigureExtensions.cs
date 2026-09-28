using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.Figures.PointGeometries;
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

        public static IEnumerable<IPointGeometry> CreateCustomShape(PointSingle startDrawingPoint,
                                                                    float pixelsPerCentimeter,
                                                                    float halfCirclesDiameterMillimeters,
                                                                    float innerHalfCirclesDiameterMillimeters,
                                                                    float distanceBetweenHalfCirclesAndLargeRectangleMillimeters,
                                                                    float verticalDistanceBetweenHalfCirclesMillimeters,
                                                                    float largeRectangleWidthMillimeters,
                                                                    float smallSquaresDimensionSizeMillimeters,
                                                                    float largeCircleDiameterMillimeters,
                                                                    float smallCircleDiameterMillimeters)
        {
            var pixelsPerMillimeter = pixelsPerCentimeter / 10f;

            var halfCirclesDiameterPixels =
                halfCirclesDiameterMillimeters * pixelsPerMillimeter;

            var innerHalfCirclesDiameterPixels =
                innerHalfCirclesDiameterMillimeters * pixelsPerMillimeter;

            var halfCirclesRadiusPixels =
                halfCirclesDiameterPixels / 2;

            var innerHalfCirclesRadiusPixels =
                innerHalfCirclesDiameterPixels / 2;

            var leftHalfCirclesStartAngleDegrees = 90f;
            var leftHalfCirclesEndAngleDegrees = 270f;

            var rightHalfCirclesStartAngleDegrees = -90f;
            var rightHalfCircleEndAngleDegrees = 90f;

            var topLeftHalfCircleCenterPoint =
                startDrawingPoint + halfCirclesDiameterPixels;

            var distanceBetweenHalfCirclesAndLargeRectanglePixels =
                distanceBetweenHalfCirclesAndLargeRectangleMillimeters
                * pixelsPerMillimeter;

            var topLeftHalfCircle =
                topLeftHalfCircleCenterPoint.GetCirclePoints(
                    halfCirclesRadiusPixels,
                    leftHalfCirclesStartAngleDegrees,
                    leftHalfCirclesEndAngleDegrees);

            var topLeftInnerCirclePoint =
                topLeftHalfCircleCenterPoint;

            var topLeftInnerCircle =
                topLeftInnerCirclePoint.GetCirclePoints(
                    innerHalfCirclesRadiusPixels);

            var verticalDistanceBetweenHalfCirclesPixels =
                verticalDistanceBetweenHalfCirclesMillimeters
                * pixelsPerMillimeter;

            var bottomLeftHalfCircleCenterPoint =
                new PointSingle(
                    x: topLeftHalfCircleCenterPoint.X,
                    y: topLeftHalfCircleCenterPoint.Y
                        + verticalDistanceBetweenHalfCirclesPixels);

            var bottomLeftHalfCircle =
                bottomLeftHalfCircleCenterPoint.GetCirclePoints(
                    halfCirclesRadiusPixels,
                    leftHalfCirclesStartAngleDegrees,
                    leftHalfCirclesEndAngleDegrees);

            var bottomLeftInnerCirclePoint =
                new PointSingle(
                    x: topLeftInnerCirclePoint.X,
                    y: topLeftInnerCirclePoint.Y
                        + verticalDistanceBetweenHalfCirclesPixels);

            var bottomLeftInnerCircle =
                bottomLeftInnerCirclePoint.GetCirclePoints(
                    innerHalfCirclesRadiusPixels);

            var topLeftHalfCircleTopLineFirstPoint =
                new PointSingle(
                    x: topLeftHalfCircleCenterPoint.X,
                    y: topLeftHalfCircleCenterPoint.Y
                        - halfCirclesRadiusPixels);

            var topLeftHalfCircleTopLineSecondPoint =
                new PointSingle(
                    x: topLeftHalfCircleCenterPoint.X
                        + distanceBetweenHalfCirclesAndLargeRectanglePixels,
                    y: topLeftHalfCircleCenterPoint.Y
                        - halfCirclesRadiusPixels);

            var bottomLeftHalfCircleTopLineFirstPoint =
                new PointSingle(
                    x: topLeftHalfCircleCenterPoint.X,
                    y: topLeftHalfCircleCenterPoint.Y
                        + halfCirclesRadiusPixels);

            var bottomLeftHalfCircleTopLineSecondPoint =
                new PointSingle(
                    x: topLeftHalfCircleCenterPoint.X
                        + distanceBetweenHalfCirclesAndLargeRectanglePixels,
                    y: topLeftHalfCircleCenterPoint.Y
                        + halfCirclesRadiusPixels);

            var bottomTopLeftHalfCircleTopLineFirstPoint =
                new PointSingle(
                    x: bottomLeftHalfCircleCenterPoint.X,
                    y: bottomLeftHalfCircleCenterPoint.Y
                        - halfCirclesRadiusPixels);

            var bottomTopLeftHalfCircleTopLineSecondPoint =
                new PointSingle(
                    x: bottomLeftHalfCircleCenterPoint.X
                        + distanceBetweenHalfCirclesAndLargeRectanglePixels,
                    y: bottomLeftHalfCircleCenterPoint.Y
                        - halfCirclesRadiusPixels);

            var bottomBottomLeftHalfCircleTopLineFirstPoint =
                new PointSingle(
                    x: bottomLeftHalfCircleCenterPoint.X,
                    y: bottomLeftHalfCircleCenterPoint.Y
                        + halfCirclesRadiusPixels);

            var bottomBottomLeftHalfCircleTopLineSecondPoint =
                new PointSingle(
                    x: bottomLeftHalfCircleCenterPoint.X
                        + distanceBetweenHalfCirclesAndLargeRectanglePixels,
                    y: bottomLeftHalfCircleCenterPoint.Y
                        + halfCirclesRadiusPixels);

            var rectangleWidthPixels =
                largeRectangleWidthMillimeters * pixelsPerMillimeter;

            var topTopRightHalfCircleTopLineSecondPoint =
                new PointSingle(
                    x: topLeftHalfCircleTopLineSecondPoint.X
                        + rectangleWidthPixels,
                    y: topLeftHalfCircleTopLineSecondPoint.Y);

            var bottomBottomRightHalfCircleTopLineFirstPoint =
                new PointSingle(
                    x: bottomBottomLeftHalfCircleTopLineSecondPoint.X,
                    y: bottomBottomLeftHalfCircleTopLineSecondPoint.Y);

            var bottomBottomRightHalfCircleTopLineSecondPoint =
                new PointSingle(
                    x: bottomBottomLeftHalfCircleTopLineSecondPoint.X
                        + rectangleWidthPixels,
                    y: bottomBottomLeftHalfCircleTopLineSecondPoint.Y);

            var smallCircleDiameterPixels =
                smallCircleDiameterMillimeters * pixelsPerMillimeter;

            var smallCircleRadiusPixels =
                smallCircleDiameterPixels / 2;

            var largeCircleDiameterPixels =
                largeCircleDiameterMillimeters * pixelsPerMillimeter;

            var largeCircleRadiusPixels =
                largeCircleDiameterPixels / 2;

            var smallSquaresDimensionSizePixels =
                smallSquaresDimensionSizeMillimeters * pixelsPerMillimeter;

            var horizontalMarginBetweenLeftRectangleBorderAndCenter =
                (rectangleWidthPixels
                 - smallSquaresDimensionSizePixels * 2
                 - largeCircleDiameterPixels) / 2;

            var figureCenterY =
                topLeftHalfCircleCenterPoint.Y
                + verticalDistanceBetweenHalfCirclesPixels / 2;

            var squareTopY =
                figureCenterY
                - smallSquaresDimensionSizePixels / 2;

            var squareBottomY =
                figureCenterY
                + smallSquaresDimensionSizePixels / 2;

            var largeCircleCenterPoint =
                new PointSingle(
                    x: topLeftHalfCircleTopLineSecondPoint.X
                        + horizontalMarginBetweenLeftRectangleBorderAndCenter
                        + smallSquaresDimensionSizePixels
                        + largeCircleRadiusPixels,

                    y: figureCenterY);

            var largeCircle =
                largeCircleCenterPoint.GetCirclePoints(
                    radius: largeCircleRadiusPixels);

            var smallCircle =
                largeCircleCenterPoint.GetCirclePoints(
                    radius: smallCircleRadiusPixels);

            var topSquareDistanceFromCircleCenter =
                squareTopY - largeCircleCenterPoint.Y;

            var bottomSquareDistanceFromCircleCenter =
                squareBottomY - largeCircleCenterPoint.Y;

            var topCircleHorizontalOffset =
                MathF.Sqrt(
                    largeCircleRadiusPixels * largeCircleRadiusPixels
                    - topSquareDistanceFromCircleCenter
                      * topSquareDistanceFromCircleCenter);

            var bottomCircleHorizontalOffset =
                MathF.Sqrt(
                    largeCircleRadiusPixels * largeCircleRadiusPixels
                    - bottomSquareDistanceFromCircleCenter
                      * bottomSquareDistanceFromCircleCenter);

            var topLeftCircleIntersectionX =
                largeCircleCenterPoint.X
                - topCircleHorizontalOffset;

            var topRightCircleIntersectionX =
                largeCircleCenterPoint.X
                + topCircleHorizontalOffset;

            var bottomLeftCircleIntersectionX =
                largeCircleCenterPoint.X
                - bottomCircleHorizontalOffset;

            var bottomRightCircleIntersectionX =
                largeCircleCenterPoint.X
                + bottomCircleHorizontalOffset;

            var topLeftSmallSquareStartPoint =
                new PointSingle(
                    x: topLeftHalfCircleTopLineSecondPoint.X
                        + horizontalMarginBetweenLeftRectangleBorderAndCenter,
                    y: squareTopY);

            var bottomLeftSmallSquareEndPoint =
                new PointSingle(
                    x: topLeftSmallSquareStartPoint.X,
                    y: squareBottomY);

            var topRightLeftSmallSquareEndPoint =
                new PointSingle(
                    x: topLeftCircleIntersectionX,
                    y: squareTopY);

            var bottomRightLeftSmallSquareEndPoint =
                new PointSingle(
                    x: bottomLeftCircleIntersectionX,
                    y: squareBottomY);

            var topLeftRightSmallSquareStartPoint =
                new PointSingle(
                    x: topRightCircleIntersectionX,
                    y: squareTopY);

            var bottomLeftRightSmallSquareEndPoint =
                new PointSingle(
                    x: bottomRightCircleIntersectionX,
                    y: squareBottomY);

            var topSmallSquareHorizontalDistance =
                topLeftCircleIntersectionX
                - topLeftSmallSquareStartPoint.X;

            var bottomSmallSquareHorizontalDistance =
                bottomLeftCircleIntersectionX
                - bottomLeftSmallSquareEndPoint.X;

            var topLeftRightSmallSquareEndPoint =
                new PointSingle(
                    x: topLeftRightSmallSquareStartPoint.X
                        + topSmallSquareHorizontalDistance,
                    y: squareTopY);

            var bottomRightSmallSquareEndPoint =
                new PointSingle(
                    x: bottomLeftRightSmallSquareEndPoint.X
                        + bottomSmallSquareHorizontalDistance,
                    y: squareBottomY);

            var topRightHalfCircleConnectionLineEndPoint =
                new PointSingle(
                    x: topTopRightHalfCircleTopLineSecondPoint.X
                        + distanceBetweenHalfCirclesAndLargeRectanglePixels,
                    y: topTopRightHalfCircleTopLineSecondPoint.Y);

            var topBottomRightHalfCircleConnectionLineStartPoint =
                new PointSingle(
                    x: topTopRightHalfCircleTopLineSecondPoint.X,
                    y: topTopRightHalfCircleTopLineSecondPoint.Y
                        + halfCirclesDiameterPixels);

            var topBottomRightHalfCircleConnectionLineEndPoint =
                new PointSingle(
                    x: topTopRightHalfCircleTopLineSecondPoint.X
                        + distanceBetweenHalfCirclesAndLargeRectanglePixels,
                    y: topTopRightHalfCircleTopLineSecondPoint.Y
                        + halfCirclesDiameterPixels);

            var topRightHalfCircleCenterPoint =
                new PointSingle(
                    x: topRightHalfCircleConnectionLineEndPoint.X,
                    y: topLeftHalfCircleCenterPoint.Y);

            var topRightHalfCircle =
                topRightHalfCircleCenterPoint.GetCirclePoints(
                    halfCirclesRadiusPixels,
                    rightHalfCirclesStartAngleDegrees,
                    rightHalfCircleEndAngleDegrees);

            var topRightInnerCircle =
                topRightHalfCircleCenterPoint.GetCirclePoints(
                    innerHalfCirclesRadiusPixels);

            var rightBorderEndPoint =
                new PointSingle(
                    x: topTopRightHalfCircleTopLineSecondPoint.X,
                    y: topTopRightHalfCircleTopLineSecondPoint.Y
                        + verticalDistanceBetweenHalfCirclesPixels);

            var bottomRightHalfCircleConnectionLineTopEndPoint =
                new PointSingle(
                    x: rightBorderEndPoint.X
                        + distanceBetweenHalfCirclesAndLargeRectanglePixels,
                    y: rightBorderEndPoint.Y);

            var bottomRightHalfCircleConnectionLineBottomStartPoint =
                new PointSingle(
                    x: rightBorderEndPoint.X,
                    y: rightBorderEndPoint.Y
                        + halfCirclesDiameterPixels);

            var bottomRightHalfCircleConnectionLineBottomEndPoint =
                new PointSingle(
                    x: bottomRightHalfCircleConnectionLineBottomStartPoint.X
                        + distanceBetweenHalfCirclesAndLargeRectanglePixels,
                    y: bottomRightHalfCircleConnectionLineBottomStartPoint.Y);

            var bottomConnectionLineEndPoint =
                new PointSingle(
                    x: rightBorderEndPoint.X,
                    y: rightBorderEndPoint.Y
                        + halfCirclesDiameterPixels);

            var bottomRightHalfCircleCenterPoint =
                new PointSingle(
                    x: bottomRightHalfCircleConnectionLineTopEndPoint.X,
                    y: bottomRightHalfCircleConnectionLineTopEndPoint.Y
                        + halfCirclesRadiusPixels);

            var bottomRightHalfCircle =
                bottomRightHalfCircleCenterPoint.GetCirclePoints(
                    halfCirclesRadiusPixels,
                    rightHalfCirclesStartAngleDegrees,
                    rightHalfCircleEndAngleDegrees);

            var bottomRightInnerCircle =
                bottomRightHalfCircleCenterPoint.GetCirclePoints(
                    innerHalfCirclesRadiusPixels);

            yield return topLeftHalfCircle.ToCirclePointGeometry();

            yield return topLeftInnerCircle.ToCirclePointGeometry();

            yield return bottomLeftInnerCircle.ToCirclePointGeometry();

            yield return bottomLeftHalfCircle.ToCirclePointGeometry();

            yield return ((ICollection<PointSingle>)[
                topLeftHalfCircleTopLineSecondPoint,
        topTopRightHalfCircleTopLineSecondPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                bottomBottomRightHalfCircleTopLineFirstPoint,
        bottomBottomRightHalfCircleTopLineSecondPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                topLeftHalfCircleTopLineSecondPoint,
        bottomLeftHalfCircleTopLineSecondPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                bottomTopLeftHalfCircleTopLineSecondPoint,
        bottomBottomLeftHalfCircleTopLineSecondPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                bottomLeftHalfCircleTopLineSecondPoint,
        bottomTopLeftHalfCircleTopLineSecondPoint])
                .ToLinePointGeometry();

            yield return largeCircle.ToCirclePointGeometry();

            yield return ((ICollection<PointSingle>)[
                topLeftSmallSquareStartPoint,
        bottomLeftSmallSquareEndPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                topLeftSmallSquareStartPoint,
        topRightLeftSmallSquareEndPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                topLeftRightSmallSquareStartPoint,
        topLeftRightSmallSquareEndPoint]).ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
        topLeftRightSmallSquareEndPoint,
        bottomRightSmallSquareEndPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
        bottomRightSmallSquareEndPoint,
        bottomLeftRightSmallSquareEndPoint])
                .ToLinePointGeometry();

            yield return smallCircle.ToCirclePointGeometry();

            yield return ((ICollection<PointSingle>)[
                bottomLeftSmallSquareEndPoint,
        bottomRightLeftSmallSquareEndPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                topTopRightHalfCircleTopLineSecondPoint,
        topRightHalfCircleConnectionLineEndPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                topBottomRightHalfCircleConnectionLineStartPoint,
        topBottomRightHalfCircleConnectionLineEndPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                topTopRightHalfCircleTopLineSecondPoint,
        topBottomRightHalfCircleConnectionLineStartPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                rightBorderEndPoint,
        bottomRightHalfCircleConnectionLineTopEndPoint])
                .ToLinePointGeometry();

            yield return topRightInnerCircle.ToCirclePointGeometry();

            yield return topRightHalfCircle.ToCirclePointGeometry();

            yield return ((ICollection<PointSingle>)[
                topBottomRightHalfCircleConnectionLineStartPoint,
        rightBorderEndPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                bottomRightHalfCircleConnectionLineBottomStartPoint,
        bottomRightHalfCircleConnectionLineBottomEndPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                topLeftHalfCircleTopLineFirstPoint,
        topLeftHalfCircleTopLineSecondPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                rightBorderEndPoint,
        bottomConnectionLineEndPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                bottomLeftHalfCircleTopLineFirstPoint,
        bottomLeftHalfCircleTopLineSecondPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                bottomTopLeftHalfCircleTopLineFirstPoint,
        bottomTopLeftHalfCircleTopLineSecondPoint])
                .ToLinePointGeometry();

            yield return ((ICollection<PointSingle>)[
                bottomBottomLeftHalfCircleTopLineFirstPoint,
        bottomBottomLeftHalfCircleTopLineSecondPoint])
                .ToLinePointGeometry();

            yield return bottomRightHalfCircle.ToCirclePointGeometry();

            yield return bottomRightInnerCircle.ToCirclePointGeometry();
        }
    }
}
