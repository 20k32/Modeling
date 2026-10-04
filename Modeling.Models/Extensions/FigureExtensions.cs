using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Abstractions.SegmentDiemnsions;
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

        public static IEnumerable<IPointGeometry> CreateCustomShape(PointSingle startDrawingPoint, ISegmentDimension dimension)
        {
            const float leftHalfCirclesStartAngleDegrees = 90f;
            const float leftHalfCirclesEndAngleDegrees = 270f;

            const float rightHalfCirclesStartAngleDegrees = -90f;
            const float rightHalfCirclesEndAngleDegrees = 90f;

            // ---------------------------------------------------------------------
            // Radii
            // ---------------------------------------------------------------------

            var topLeftHalfCircleRadius =
                dimension.TopLeftHalfCircleDiameter / 2f;

            var topLeftInnerCircleRadius =
                dimension.TopLeftInnerCircleDiameter / 2f;

            var bottomLeftHalfCircleRadius =
                dimension.BottomLeftHalfCircleDiameter / 2f;

            var bottomLeftInnerCircleRadius =
                dimension.BottomLeftInnerCircleDiameter / 2f;

            var largeCircleRadius =
                dimension.LargeCircleDiameter / 2f;

            var smallCircleRadius =
                dimension.SmallCircleDiameter / 2f;

            var topRightHalfCircleRadius =
                dimension.TopRightHalfCircleDiameter / 2f;

            var topRightInnerCircleRadius =
                dimension.TopRightInnerCircleDiameter / 2f;

            var bottomRightHalfCircleRadius =
                dimension.BottomRightHalfCircleDiameter / 2f;

            var bottomRightInnerCircleRadius =
                dimension.BottomRightInnerCircleDiameter / 2f;

            // ---------------------------------------------------------------------
            // LEFT SIDE - TOP HALF CIRCLE
            // ---------------------------------------------------------------------

            var topLeftHalfCircleCenterPoint =
                startDrawingPoint
                + dimension.TopLeftHalfCircleDiameter;

            var topLeftHalfCircleTopLineFirstPoint =
                new PointSingle(
                    x: topLeftHalfCircleCenterPoint.X,
                    y: topLeftHalfCircleCenterPoint.Y
                        - topLeftHalfCircleRadius);

            var topLeftHalfCircleTopLineSecondPoint =
                new PointSingle(
                    x: topLeftHalfCircleTopLineFirstPoint.X
                        + dimension.TopTopLeftHalfCircleTopLineLength,
                    y: topLeftHalfCircleTopLineFirstPoint.Y);

            var topLeftHalfCircleBottomLineFirstPoint = new PointSingle(
                x: topLeftHalfCircleTopLineFirstPoint.X,
                y: topLeftHalfCircleTopLineFirstPoint.Y + dimension.TopTopLeftHalfCircleTopLineLength);

            var topLeftHalfCircleBottomLineSecondPoint = new PointSingle(
                x: topLeftHalfCircleTopLineFirstPoint.X + dimension.TopBottomLeftHalfCircleTopLineLength,
                y: topLeftHalfCircleTopLineFirstPoint.Y + dimension.TopTopLeftHalfCircleTopLineLength);

            // ---------------------------------------------------------------------
            // LEFT SIDE - BOTTOM HALF CIRCLE
            // ---------------------------------------------------------------------

            // The complete main rectangle height is 5 cm.
            //
            // The existing dimension model stores this as:
            //
            // LeftUpperVerticalLineLength
            // + LeftMiddleVerticalLineLength
            //
            // Therefore the center of the main rectangle is halfway through
            // these two dimensions.

            var largeRectangleHeight = dimension.LeftMiddleVerticalLineLength;

            var largeRectangleTopY =
                topLeftHalfCircleTopLineSecondPoint.Y;

            var largeRectangleBottomY =
                largeRectangleTopY + largeRectangleHeight;

            var largeRectangleCenterY =
                largeRectangleTopY + largeRectangleHeight / 2f;

            // problem 1 dimension.BottomLeftHalfCircleTopLineLength
            var bottomLeftHalfCircleCenterPoint =
                new PointSingle(
                    x: topLeftHalfCircleCenterPoint.X,
                    y: topLeftHalfCircleBottomLineFirstPoint.Y + largeRectangleHeight);

            var bottomLeftHalfCircleTopLineFirstPoint =
                new PointSingle(
                    x: topLeftHalfCircleBottomLineFirstPoint.X,
                    y: topLeftHalfCircleBottomLineFirstPoint.Y
                        + largeRectangleHeight);

            var bottomLeftHalfCircleTopLineSecondPoint =
                new PointSingle(
                    x: bottomLeftHalfCircleTopLineFirstPoint.X + dimension.BottomBottomLeftHalfCircleTopLineLength,
                    y: bottomLeftHalfCircleTopLineFirstPoint.Y + dimension.BottomRightVerticalLineLength);

            // ---------------------------------------------------------------------
            // LEFT SIDE - INNER CIRCLES
            // ---------------------------------------------------------------------

            var topLeftInnerCircleCenterPoint =
                topLeftHalfCircleCenterPoint;

            var bottomLeftInnerCircleCenterPoint =
                bottomLeftHalfCircleCenterPoint;

            // ---------------------------------------------------------------------
            // LEFT SIDE - LOWER CONNECTING HALF CIRCLES
            // ---------------------------------------------------------------------

            var bottomTopLeftHalfCircleTopLineFirstPoint =
                new PointSingle(
                    x: bottomLeftHalfCircleCenterPoint.X,
                    y: bottomLeftHalfCircleCenterPoint.Y
                        - bottomLeftHalfCircleRadius);

            var bottomTopLeftHalfCircleTopLineSecondPoint =
                new PointSingle(
                    x: bottomTopLeftHalfCircleTopLineFirstPoint.X
                        + dimension.BottomTopLeftHalfCircleTopLineLength,
                    y: bottomTopLeftHalfCircleTopLineFirstPoint.Y);

            var bottomBottomLeftHalfCircleTopLineFirstPoint =
                new PointSingle(
                    x: bottomLeftHalfCircleCenterPoint.X,
                    y: bottomLeftHalfCircleCenterPoint.Y
                        + bottomLeftHalfCircleRadius);

            var bottomBottomLeftHalfCircleTopLineSecondPoint =
                new PointSingle(
                    x: bottomBottomLeftHalfCircleTopLineFirstPoint.X
                        + dimension.BottomBottomLeftHalfCircleTopLineLength,
                    y: bottomBottomLeftHalfCircleTopLineFirstPoint.Y);

            // ---------------------------------------------------------------------
            // MAIN RECTANGLE
            // ---------------------------------------------------------------------

            var largeRectangleLeftX =
                topLeftHalfCircleTopLineSecondPoint.X;

            var largeRectangleRightX =
                largeRectangleLeftX
                + dimension.TopLeftHorizontalLineLength;

            // The main rectangle is 9 cm wide.
            //
            // The large circle is centered horizontally using the actual
            // rectangle bounds, not using the adjacent rectangle dimensions.

            var largeRectangleCenterX =
                (largeRectangleLeftX + largeRectangleRightX) / 2f;

            // ---------------------------------------------------------------------
            // LARGE CIRCLE
            // ---------------------------------------------------------------------

            var largeCircleCenterPoint =
                new PointSingle(
                    x: largeRectangleCenterX,
                    y: largeRectangleCenterY);

            // ---------------------------------------------------------------------
            // SQUARE / RECTANGLE VERTICAL POSITIONS
            // ---------------------------------------------------------------------

            var squareTopY =
                largeRectangleCenterY
                - dimension.LeftSquareVerticalLineLength / 2f;

            var squareBottomY =
                largeRectangleCenterY
                + dimension.LeftSquareVerticalLineLength / 2f;

            // ---------------------------------------------------------------------
            // LARGE CIRCLE / RECTANGLE INTERSECTIONS
            //
            // These points determine where the adjacent rectangles touch the
            // large circle.
            // ---------------------------------------------------------------------

            var topCircleDistanceFromCenter =
                squareTopY - largeCircleCenterPoint.Y;

            var bottomCircleDistanceFromCenter =
                squareBottomY - largeCircleCenterPoint.Y;

            var topCircleHorizontalOffset =
                MathF.Sqrt(
                    MathF.Max(
                        0f,
                        largeCircleRadius * largeCircleRadius
                        - topCircleDistanceFromCenter
                          * topCircleDistanceFromCenter));

            var bottomCircleHorizontalOffset =
                MathF.Sqrt(
                    MathF.Max(
                        0f,
                        largeCircleRadius * largeCircleRadius
                        - bottomCircleDistanceFromCenter
                          * bottomCircleDistanceFromCenter));

            var topLeftCircleIntersectionPoint =
                new PointSingle(
                    x: largeCircleCenterPoint.X
                        - topCircleHorizontalOffset,
                    y: squareTopY);

            var topRightCircleIntersectionPoint =
                new PointSingle(
                    x: largeCircleCenterPoint.X
                        + topCircleHorizontalOffset,
                    y: squareTopY);

            var bottomLeftCircleIntersectionPoint =
                new PointSingle(
                    x: largeCircleCenterPoint.X
                        - bottomCircleHorizontalOffset,
                    y: squareBottomY);

            var bottomRightCircleIntersectionPoint =
                new PointSingle(
                    x: largeCircleCenterPoint.X
                        + bottomCircleHorizontalOffset,
                    y: squareBottomY);

            // ---------------------------------------------------------------------
            // LEFT ADJACENT RECTANGLE
            //
            // Its right side is defined by the large circle intersection points.
            // Therefore it is physically adjacent to the large circle.
            // ---------------------------------------------------------------------

            var topLeftSmallSquareStartPoint =
                new PointSingle(
                    x: largeRectangleLeftX,
                    y: squareTopY);

            var topRightLeftSmallSquareEndPoint =
                topLeftCircleIntersectionPoint;

            var bottomLeftSmallSquareEndPoint =
                new PointSingle(
                    x: largeRectangleLeftX,
                    y: squareBottomY);

            var bottomRightLeftSmallSquareEndPoint =
                bottomLeftCircleIntersectionPoint;

            // ---------------------------------------------------------------------
            // RIGHT ADJACENT RECTANGLE
            //
            // Its left side is defined by the large circle intersection points.
            // Therefore it is physically adjacent to the large circle.
            // ---------------------------------------------------------------------

            var topLeftRightSmallSquareStartPoint =
                topRightCircleIntersectionPoint;

            var topLeftRightSmallSquareEndPoint =
                new PointSingle(
                    x: largeRectangleRightX,
                    y: squareTopY);

            var bottomLeftRightSmallSquareStartPoint =
                bottomRightCircleIntersectionPoint;

            var bottomRightRightSmallSquareEndPoint =
                new PointSingle(
                    x: largeRectangleRightX,
                    y: squareBottomY);

            // ---------------------------------------------------------------------
            // BOTTOM LEFT RECTANGLE
            // ---------------------------------------------------------------------

            var bottomLeftBottomSmallSquareStartPoint =
                bottomLeftCircleIntersectionPoint;

            var bottomRightBottomSmallSquareEndPoint =
                new PointSingle(
                    x: bottomLeftBottomSmallSquareStartPoint.X
                        + dimension.BottomLeftSquareHorizontalLineLength,
                    y: bottomLeftBottomSmallSquareStartPoint.Y);

            // ---------------------------------------------------------------------
            // BOTTOM RIGHT RECTANGLE
            // ---------------------------------------------------------------------

            var bottomLeftRightBottomSmallSquareStartPoint =
                bottomRightCircleIntersectionPoint;

            var bottomRightRightBottomSmallSquareEndPoint =
                new PointSingle(
                    x: bottomLeftRightBottomSmallSquareStartPoint.X
                        + dimension.BottomSquareHorizontalLineLength,
                    y: bottomLeftRightBottomSmallSquareStartPoint.Y);

            // ---------------------------------------------------------------------
            // SMALL CIRCLE
            //
            // Center it inside the RIGHT adjacent rectangle.
            // ---------------------------------------------------------------------

            var smallCircleCenterPoint =
                new PointSingle(
                    x: (
                        topLeftRightSmallSquareStartPoint.X
                        + topLeftRightSmallSquareEndPoint.X
                    ) / 2f,
                    y: (
                        topLeftRightSmallSquareStartPoint.Y
                        + bottomRightRightSmallSquareEndPoint.Y
                    ) / 2f);

            // ---------------------------------------------------------------------
            // TOP RIGHT SIDE
            // ---------------------------------------------------------------------

            var topTopRightHalfCircleTopLineFirstPoint =
                new PointSingle(
                    x: largeRectangleRightX,
                    y: largeRectangleTopY);

            var topTopRightHalfCircleTopLineSecondPoint =
                new PointSingle(
                    x: topTopRightHalfCircleTopLineFirstPoint.X
                        + dimension.TopRightConnectionLineLength,
                    y: topTopRightHalfCircleTopLineFirstPoint.Y);

            var topRightVerticalLineStartPoint =
                new PointSingle(
                    x: topTopRightHalfCircleTopLineFirstPoint.X,
                    y: topTopRightHalfCircleTopLineFirstPoint.Y
                        + dimension.TopRightVerticalLineLength);

            var topRightLowerConnectionLineEndPoint =
                new PointSingle(
                    x: topRightVerticalLineStartPoint.X
                        + dimension.TopRightLowerConnectionLineLength,
                    y: topRightVerticalLineStartPoint.Y);

            var topRightHalfCircleCenterPoint =
                new PointSingle(
                    x: topRightLowerConnectionLineEndPoint.X,
                    y: topLeftHalfCircleCenterPoint.Y);

            // ---------------------------------------------------------------------
            // RIGHT BORDER
            // ---------------------------------------------------------------------

            var rightBorderEndPoint =
                new PointSingle(
                    x: topTopRightHalfCircleTopLineFirstPoint.X,
                    y: topTopRightHalfCircleTopLineFirstPoint.Y
                        + dimension.TopRightVerticalLineLength
                        + dimension.RightMiddleVerticalLineLength);

            // ---------------------------------------------------------------------
            // BOTTOM RIGHT SECTION
            // ---------------------------------------------------------------------

            var bottomRightHalfCircleTopLineFirstPoint =
                new PointSingle(
                    x: rightBorderEndPoint.X
                        + dimension.TopLeftCircleConnectionLineLength,
                    y: rightBorderEndPoint.Y);

            var bottomRightHalfCircleBottomLineFirstPoint =
                new PointSingle(
                    x: bottomRightHalfCircleTopLineFirstPoint.X,
                    y: bottomRightHalfCircleTopLineFirstPoint.Y
                        + dimension.BottomRightVerticalLineLength);

            var bottomRightHalfCircleBottomLineSecondPoint =
                new PointSingle(
                    x: bottomRightHalfCircleBottomLineFirstPoint.X
                        + dimension.BottomRightConnectionLineLength,
                    y: bottomRightHalfCircleBottomLineFirstPoint.Y);

            var bottomRightHalfCircleCenterPoint =
                new PointSingle(
                    x: bottomRightHalfCircleTopLineFirstPoint.X,
                    y: bottomRightHalfCircleTopLineFirstPoint.Y
                        + bottomRightHalfCircleRadius);

            // ---------------------------------------------------------------------
            // MAIN RECTANGLE - BOTTOM
            // ---------------------------------------------------------------------

            var bottomBottomRightHalfCircleTopLineFirstPoint =
                new PointSingle(
                    x: bottomLeftHalfCircleTopLineSecondPoint.X
                        + dimension.BottomHorizontalLineLength,
                    y: bottomLeftHalfCircleTopLineSecondPoint.Y);

            var bottomBottomRightHalfCircleTopLineSecondPoint =
                new PointSingle(
                    x: bottomBottomRightHalfCircleTopLineFirstPoint.X
                        + dimension.BottomRightConnectionLineLength,
                    y: bottomBottomRightHalfCircleTopLineFirstPoint.Y);

            // ---------------------------------------------------------------------
            // GEOMETRY
            // ---------------------------------------------------------------------

            // Top-left half circle
            yield return topLeftHalfCircleCenterPoint
                .GetCirclePoints(
                    topLeftHalfCircleRadius,
                    startAngle: leftHalfCirclesStartAngleDegrees,
                    endAngle: leftHalfCirclesEndAngleDegrees)
                .ToCirclePointGeometry(
                    topLeftHalfCircleCenterPoint,
                    SegmentDimensionParameter.TopLeftHalfCircleDiameter);

            // Top-left inner circle
            yield return topLeftInnerCircleCenterPoint
                .GetCirclePoints(topLeftInnerCircleRadius)
                .ToCirclePointGeometry(
                    topLeftInnerCircleCenterPoint,
                    SegmentDimensionParameter.TopLeftInnerCircleDiameter);

            // Bottom-left inner circle
            yield return bottomLeftInnerCircleCenterPoint
                .GetCirclePoints(bottomLeftInnerCircleRadius)
                .ToCirclePointGeometry(
                    bottomLeftInnerCircleCenterPoint,
                    SegmentDimensionParameter.BottomLeftInnerCircleDiameter);

            // Bottom-left half circle
            yield return bottomLeftHalfCircleCenterPoint
                .GetCirclePoints(
                    bottomLeftHalfCircleRadius,
                    startAngle: leftHalfCirclesStartAngleDegrees,
                    endAngle: leftHalfCirclesEndAngleDegrees)
                .ToCirclePointGeometry(
                    bottomLeftHalfCircleCenterPoint,
                    SegmentDimensionParameter.BottomLeftHalfCircleDiameter);

            // Top main rectangle line
            yield return ((ICollection<PointSingle>)[
                topLeftHalfCircleTopLineSecondPoint,
        topTopRightHalfCircleTopLineFirstPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.TopHorizontalLineLength);

            // Bottom main rectangle line
            yield return ((ICollection<PointSingle>)[
                bottomLeftHalfCircleTopLineSecondPoint,
        bottomBottomRightHalfCircleTopLineFirstPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.BottomHorizontalLineLength);

            // Top left half circle bottom line
            yield return ((ICollection<PointSingle>)[
                topLeftHalfCircleBottomLineFirstPoint,
        topLeftHalfCircleBottomLineSecondPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.LeftUpperVerticalLineLength);

            // Left middle vertical problem 2
            yield return ((ICollection<PointSingle>)[
                topLeftHalfCircleBottomLineSecondPoint,
        bottomLeftHalfCircleTopLineSecondPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.LeftMiddleVerticalLineLength);

            // Left lower vertical
            yield return ((ICollection<PointSingle>)[
                bottomLeftHalfCircleCenterPoint,
        bottomLeftHalfCircleTopLineSecondPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.LeftLowerVerticalLineLength);

            // Large circle
            yield return largeCircleCenterPoint
                .GetCirclePoints(largeCircleRadius)
                .ToCirclePointGeometry(
                    largeCircleCenterPoint,
                    SegmentDimensionParameter.LargeCircleDiameter);

            // ---------------------------------------------------------------------
            // LEFT ADJACENT RECTANGLE
            // ---------------------------------------------------------------------

            yield return ((ICollection<PointSingle>)[
                topLeftSmallSquareStartPoint,
        topRightLeftSmallSquareEndPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.LeftSquareHorizontalLineLength);

            yield return ((ICollection<PointSingle>)[
                topRightLeftSmallSquareEndPoint,
        bottomRightLeftSmallSquareEndPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.LeftSquareVerticalLineLength);

            yield return ((ICollection<PointSingle>)[
                bottomLeftSmallSquareEndPoint,
        bottomRightLeftSmallSquareEndPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.LeftSquareHorizontalLineLength);

            yield return ((ICollection<PointSingle>)[
                topLeftSmallSquareStartPoint,
        bottomLeftSmallSquareEndPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.LeftSquareVerticalLineLength);

            // ---------------------------------------------------------------------
            // RIGHT ADJACENT RECTANGLE
            // ---------------------------------------------------------------------

            yield return ((ICollection<PointSingle>)[
                topLeftRightSmallSquareStartPoint,
        topLeftRightSmallSquareEndPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.RightSquareHorizontalLineLength);

            yield return ((ICollection<PointSingle>)[
                topLeftRightSmallSquareEndPoint,
        bottomRightRightSmallSquareEndPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.RightSquareVerticalLineLength);

            yield return ((ICollection<PointSingle>)[
                bottomLeftRightSmallSquareStartPoint,
        bottomRightRightSmallSquareEndPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.BottomSquareHorizontalLineLength);

            // Small circle
            yield return smallCircleCenterPoint
                .GetCirclePoints(smallCircleRadius)
                .ToCirclePointGeometry(
                    smallCircleCenterPoint,
                    SegmentDimensionParameter.SmallCircleDiameter);

            // Bottom-left rectangle
            yield return ((ICollection<PointSingle>)[
                bottomLeftBottomSmallSquareStartPoint,
        bottomRightBottomSmallSquareEndPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.BottomLeftSquareHorizontalLineLength);

            // ---------------------------------------------------------------------
            // TOP RIGHT
            // ---------------------------------------------------------------------

            yield return ((ICollection<PointSingle>)[
                topTopRightHalfCircleTopLineFirstPoint,
        topTopRightHalfCircleTopLineSecondPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.TopRightConnectionLineLength);

            yield return ((ICollection<PointSingle>)[
                topRightVerticalLineStartPoint,
        topRightLowerConnectionLineEndPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.TopRightLowerConnectionLineLength);

            yield return ((ICollection<PointSingle>)[
                topTopRightHalfCircleTopLineFirstPoint,
        topRightVerticalLineStartPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.TopRightVerticalLineLength);

            yield return topRightHalfCircleCenterPoint
                .GetCirclePoints(
                    topRightHalfCircleRadius,
                    startAngle: rightHalfCirclesStartAngleDegrees,
                    endAngle: rightHalfCirclesEndAngleDegrees)
                .ToCirclePointGeometry(
                    topRightHalfCircleCenterPoint,
                    SegmentDimensionParameter.TopRightHalfCircleDiameter);

            yield return topRightHalfCircleCenterPoint
                .GetCirclePoints(topRightInnerCircleRadius)
                .ToCirclePointGeometry(
                    topRightHalfCircleCenterPoint,
                    SegmentDimensionParameter.TopRightInnerCircleDiameter);

            // ---------------------------------------------------------------------
            // RIGHT SIDE
            // ---------------------------------------------------------------------

            yield return ((ICollection<PointSingle>)[
                topRightVerticalLineStartPoint,
        rightBorderEndPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.RightMiddleVerticalLineLength);

            yield return ((ICollection<PointSingle>)[
                rightBorderEndPoint,
        bottomRightHalfCircleTopLineFirstPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.TopLeftCircleConnectionLineLength);

            // ---------------------------------------------------------------------
            // BOTTOM RIGHT
            // ---------------------------------------------------------------------

            yield return ((ICollection<PointSingle>)[
                bottomRightHalfCircleTopLineFirstPoint,
        bottomRightHalfCircleBottomLineFirstPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.BottomRightVerticalLineLength);

            yield return ((ICollection<PointSingle>)[
                bottomRightHalfCircleBottomLineFirstPoint,
        bottomRightHalfCircleBottomLineSecondPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.BottomRightConnectionLineLength);

            yield return bottomRightHalfCircleCenterPoint
                .GetCirclePoints(
                    bottomRightHalfCircleRadius,
                    startAngle: rightHalfCirclesStartAngleDegrees,
                    endAngle: rightHalfCirclesEndAngleDegrees)
                .ToCirclePointGeometry(
                    bottomRightHalfCircleCenterPoint,
                    SegmentDimensionParameter.BottomRightHalfCircleDiameter);

            yield return bottomRightHalfCircleCenterPoint
                .GetCirclePoints(bottomRightInnerCircleRadius)
                .ToCirclePointGeometry(
                    bottomRightHalfCircleCenterPoint,
                    SegmentDimensionParameter.BottomRightInnerCircleDiameter);

            // ---------------------------------------------------------------------
            // LEFT HALF-CIRCLE CONNECTION LINES
            // ---------------------------------------------------------------------

            yield return ((ICollection<PointSingle>)[
                topLeftHalfCircleTopLineFirstPoint,
        topLeftHalfCircleTopLineSecondPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.TopLeftHalfCircleTopLineLength);

            yield return ((ICollection<PointSingle>)[
                bottomLeftHalfCircleTopLineFirstPoint,
        bottomLeftHalfCircleTopLineSecondPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.BottomLeftHalfCircleTopLineLength);

            yield return ((ICollection<PointSingle>)[
                bottomTopLeftHalfCircleTopLineFirstPoint,
        bottomTopLeftHalfCircleTopLineSecondPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.BottomTopLeftHalfCircleTopLineLength);

            yield return ((ICollection<PointSingle>)[
                bottomBottomLeftHalfCircleTopLineFirstPoint,
        bottomBottomLeftHalfCircleTopLineSecondPoint
            ]).ToLinePointGeometry(
                SegmentDimensionParameter.BottomBottomLeftHalfCircleTopLineLength);
        }
    }
}
