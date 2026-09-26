using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using System.Collections.Generic;
using Windows.Graphics;

namespace Modeling.Models.Extensions
{
    public static class FigureExtensions
    {
        public static IEnumerable<PointSingle> InitializeGrid(SizeInt32 canvasSize, int pixelsPerCentimeter)
        {
            for (var y = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                y <= canvasSize.Height; y += (int)pixelsPerCentimeter)
            {
                for (var x = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                    x <= canvasSize.Width + pixelsPerCentimeter; x += (int)pixelsPerCentimeter)
                {
                    yield return new PointSingle(x, y);
                }

                if (y < canvasSize.Height)
                {
                    yield return DrawingConstants.INVALID_POINT;
                }
            }

            for (var x = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                x <= canvasSize.Width; x += (int)pixelsPerCentimeter)
            {
                for (var y = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                    y <= canvasSize.Height + pixelsPerCentimeter; y += (int)pixelsPerCentimeter)
                {
                    yield return new PointSingle(x, y);
                }

                if (x < canvasSize.Width)
                {
                    yield return DrawingConstants.INVALID_POINT;
                }
            }
        }

        public static IEnumerable<PointSingle> InitializeAxisLine(int dimensionSize, int pixelsPerCentimeter, float centerX, float centerY, bool isVertical)
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

            yield return DrawingConstants.INVALID_POINT;

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

            yield return DrawingConstants.INVALID_POINT;
        }

        public static IEnumerable<PointSingle> InitializeArrowHead(PointSingle endPoint, bool isVertical, float arrowHeadSize, bool pointingLeft = false, bool pointingUp = false)
        {
            yield return DrawingConstants.INVALID_POINT;

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

            yield return DrawingConstants.INVALID_POINT;
        }

        public static IEnumerable<PointSingle> InitializeAxisMarks(int dimensionSize, int pixelsPerCentimeter, float tickStart, float tickEnd, float centerX, float centerY, bool isVertical)
        {
            if (isVertical)
            {
                for (var axisPosition = centerY; axisPosition < dimensionSize; axisPosition += pixelsPerCentimeter)
                {
                    for (var tickOffset = tickStart; tickOffset <= tickEnd; tickOffset++)
                    {
                        yield return new PointSingle(centerX + tickOffset, axisPosition);
                    }

                    yield return DrawingConstants.INVALID_POINT;
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

                    yield return DrawingConstants.INVALID_POINT;
                }
            }

            yield return DrawingConstants.INVALID_POINT;

            if (isVertical)
            {
                for (var axisPosition = centerY - pixelsPerCentimeter; axisPosition > 0; axisPosition -= pixelsPerCentimeter)
                {
                    for (var tickOffset = tickStart; tickOffset <= tickEnd; tickOffset++)
                    {
                        yield return new PointSingle(centerX + tickOffset, axisPosition);
                    }

                    yield return DrawingConstants.INVALID_POINT;
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
                    yield return DrawingConstants.INVALID_POINT;
                }
            }

            yield return DrawingConstants.INVALID_POINT;
        }
    }
}
