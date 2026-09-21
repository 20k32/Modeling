using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Drawing.Providers;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Core.Messages.Canvas.Settings;
using Modeling.Core.Messages.Parameters.Canvas;
using Modeling.Core.Messages.Settings;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.ViewModels.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.ViewModels
{
    public sealed partial class DrawingViewModel : ObservableObject
    {
        const int USER_POINT_DRAWING_TIMEOUT_MILISECONDS = 100;

        static readonly PointSingle START_DRAWING_POINT = new PointSingle(769, 35);

        readonly IDrawingSettingsProvider _drawingSettingsProvider;

        readonly List<PointSingle> _grid;
        readonly List<PointSingle> _horizontalAxis;
        readonly List<PointSingle> _verticalAxis;
        readonly List<PointSingle> _horizontalAxisMarks;
        readonly List<PointSingle> _verticalAxisMarks;
        readonly List<PointSingle> _userPoint;
        readonly IFigure _figure;

        bool _drawUserPoint;
        PointSingle _previousMovedPoint;
        Matrix3x3Single _transform;

        CancellationTokenSource _mouseMovingCancellationSource;

        PointListTransformMessageParameter _gridWithAxisDrawingMessage;

        public DrawingViewModel()
        {
            _figure = Ioc.Default.GetRequiredService<IFigure>();
            _grid = [];

            _horizontalAxis = [];
            _horizontalAxisMarks = [];

            _verticalAxis = [];
            _verticalAxisMarks = [];

            _userPoint = [];

            _transform = DrawingConstants.NON_TRANSFORM_MATRIX;

            _drawingSettingsProvider = Ioc.Default.GetRequiredService<IDrawingSettingsProvider>();
        }

        [RelayCommand]
        void Initialize()
        {
            Logger.LoadedInformation("Main page");

            InitializeCanvas();

            InitializeDrawingSession();

            InitializeSettings();
        }

        [RelayCommand]
        void DrawLines()
        {
            DrawGrid();
        }

        [RelayCommand]
        async Task CanvasPointerMoved(PointSingle point)
        {
            if (!_drawUserPoint)
            {
                return;
            }

            try
            {
                await _mouseMovingCancellationSource.TryCancelAsync(shouldDispose: false);

                _mouseMovingCancellationSource = default;

                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    await CanvasPointerMovedCoreAsync(point, cancellationTokenSource);
                }
            }
            catch (Exception ex)
            {
                if (ex is not OperationCanceledException)
                {
                    Logger.Exception(ex);
                }
            }
        }

        async Task CanvasPointerMovedCoreAsync(PointSingle point, CancellationTokenSource cancellationTokenSource)
        {
            _mouseMovingCancellationSource = cancellationTokenSource;

            await Task.Delay(USER_POINT_DRAWING_TIMEOUT_MILISECONDS, cancellationTokenSource.Token);

            RedrawUserPoint(point);
        }

        void RedrawUserPoint(PointSingle point)
        {
            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;

            var thickness = _drawingSettingsProvider.Settings.GridDrawingThickness;

            var rawBackgroundColor = _drawingSettingsProvider.Settings.BackgroundColor;
            var backgroundColor = new DrawingColor(rawBackgroundColor);

            var circleColor = _drawingSettingsProvider.Settings.VerticalAxisColor;
            var circleDrawingColor = new DrawingColor(circleColor);

            if (point != _previousMovedPoint)
            {
                var centerX = canvasSize.Width / 2f;
                var centerY = canvasSize.Height / 2f;

                var offsetX = point.X - centerX;
                var offsetY = point.Y - centerY;

                var circleTransform = MatrixExtensions.CreateTranslationTransform(
                    offsetX,
                    offsetY);

                var drawCircleMessageParameter = new PointListTransformMessageParameter(
                    points: _userPoint,
                    color: circleDrawingColor,
                    transformMatrix: circleTransform,
                    shouldFillGeometry: true,
                    fillColor: new DrawingColor(_drawingSettingsProvider.Settings.HorizontalAxisColor),
                    clearBeforeRedraw: false,
                    backgroundColor: backgroundColor,
                    thickness: thickness,
                    parent: _gridWithAxisDrawingMessage);

                WeakReferenceMessenger.Default.Send(new TransformPointsMessage(this, drawCircleMessageParameter));

                _previousMovedPoint = point;
            }
        }

        void DrawGrid()
        {
            _grid.Clear();

            _horizontalAxis.Clear();
            _horizontalAxisMarks.Clear();

            _verticalAxis.Clear();
            _verticalAxisMarks.Clear();

            _userPoint.Clear();
            _figure.Clear();

            InitializeGrid();
            InitializeAxis();
            InitializeMarksOnAxis();
            InitializeCircle();
            InitializeFigure();

            var rawBackgroundColor = _drawingSettingsProvider.Settings.BackgroundColor;
            var backgroundColor = new DrawingColor(rawBackgroundColor);

            var rawDrawingColor = _drawingSettingsProvider.Settings.DrawingColor;
            var drawingColor = new DrawingColor(rawDrawingColor);

            var horizontalAxisColor = _drawingSettingsProvider.Settings.HorizontalAxisColor;
            var horizontalAxisDrawingColor = new DrawingColor(horizontalAxisColor);

            var verticalAxisColor = _drawingSettingsProvider.Settings.VerticalAxisColor;
            var verticalAxisDrawingColor = new DrawingColor(verticalAxisColor);

            var horizontalAxisTicksColor = _drawingSettingsProvider.Settings.HorizontalAxisTicksColor;
            var horizontalAxisTicksDrawingColor = new DrawingColor(horizontalAxisTicksColor);

            var verticalAxisTicksColor = _drawingSettingsProvider.Settings.VerticalAxisTicksColor;
            var verticalAxisTicksDrawingColor = new DrawingColor(verticalAxisTicksColor);

            var thickness = _drawingSettingsProvider.Settings.GridDrawingThickness;
            var axisThickness = _drawingSettingsProvider.Settings.AxisThickness;
            var axisTickThickness = _drawingSettingsProvider.Settings.AxisTickThickness;

            var drawGridMessageParameter = new PointListTransformMessageParameter(
                points: _grid,
                color: drawingColor,
                _transform,
                clearBeforeRedraw: true,
                backgroundColor: backgroundColor,
                thickness: thickness);

            var drawHorizontalAxisMessageParameter = new PointListTransformMessageParameter(
                points: _horizontalAxis,
                color: horizontalAxisDrawingColor,
                transformMatrix: _transform,
                shouldFillGeometry: false,
                fillColor: default,
                clearBeforeRedraw: false,
                backgroundColor: backgroundColor,
                thickness: axisThickness,
                parent: drawGridMessageParameter);

            var drawVerticalAxisMessageParameter = new PointListTransformMessageParameter(
                points: _verticalAxis,
                color: verticalAxisDrawingColor,
                transformMatrix: _transform,
                shouldFillGeometry: false,
                fillColor: default,
                clearBeforeRedraw: false,
                backgroundColor: backgroundColor,
                thickness: axisThickness,
                parent: drawHorizontalAxisMessageParameter);

            var drawHorizontalAxisMarksMessageParameter = new PointListTransformMessageParameter(
                points: _horizontalAxisMarks,
                color: horizontalAxisTicksDrawingColor,
                transformMatrix: _transform,
                shouldFillGeometry: false,
                fillColor: default,
                clearBeforeRedraw: false,
                backgroundColor: backgroundColor,
                thickness: axisTickThickness,
                parent: drawVerticalAxisMessageParameter);

            var drawVerticalAxisMarksMessageParameter = new PointListTransformMessageParameter(
                points: _verticalAxisMarks,
                color: verticalAxisTicksDrawingColor,
                transformMatrix: _transform,
                shouldFillGeometry: false,
                fillColor: default,
                clearBeforeRedraw: false,
                backgroundColor: backgroundColor,
                thickness: axisTickThickness,
                parent: drawHorizontalAxisMarksMessageParameter);

            var figureDrawingMessage = GetDrawingFigureMessage(drawVerticalAxisMarksMessageParameter);

            WeakReferenceMessenger.Default.Send(new TransformPointsMessage(this, figureDrawingMessage));

            _gridWithAxisDrawingMessage = drawVerticalAxisMarksMessageParameter;
        }

        private void AddPointsToFigureAsSegment(IEnumerable<PointSingle> points)
        {
            var segment = Ioc.Default.GetRequiredService<IPointGeometry>();
            segment.AddPointsRange(points);
            _figure.AddSegment(segment);
        }

        // accordig var 14
        // assume that figure will be drawn at the second quarter of graphic
        // canvas coords of second quarter approx x: 769 y: 35
        // don't matter in this 'example' cause we will use transforms
        //todo: take into account pixelsPerMillimeter 
        private void InitializeFigure()
        {
            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;
            var pixelsPerMillimeter = pixelsPerCentimeter / 10f;

            var halfCirclesDiameterMillimeters = FirgureRelatedConstants.HALF_CIRCLES_DIAMETER_MILLIMETERS * pixelsPerMillimeter;
            var innerHalfCirclesDiameterMillimeters = FirgureRelatedConstants.INNER_HALF_CIRCLES_DIAMETER_MILLIMETERS * pixelsPerMillimeter;

            var halfCirclesRadiusMillimeters = halfCirclesDiameterMillimeters / 2;
            var innerHalfCirclesRadiusMillimeters = innerHalfCirclesDiameterMillimeters / 2;

            var leftHalfCirclesStartAngleDegrees = 90f;
            var leftHalfCirclesEndAngleDegrees = 270f;

            var rightHalfCirclesStartAngleDegrees = -90f;
            var rightHalfCircleEndAngleDegrees = 90f;

            var topLeftHalfCircleCenterPoint = START_DRAWING_POINT + halfCirclesDiameterMillimeters;

            var distanceBetweenHalfCirclesAndLargeRectangleMillimeters = FirgureRelatedConstants.DISTANCE_BETWEEN_HALF_CIRCLES_AND_LARGE_RECTANGLE * pixelsPerMillimeter;


            var topLeftHalfCircle = topLeftHalfCircleCenterPoint.GetCirclePoints(halfCirclesRadiusMillimeters,
                leftHalfCirclesStartAngleDegrees,
                leftHalfCirclesEndAngleDegrees);

            AddPointsToFigureAsSegment(topLeftHalfCircle);

            var topLeftInnerCirclePoint = topLeftHalfCircleCenterPoint;

            var topLeftInnerCircle = topLeftInnerCirclePoint.GetCirclePoints(innerHalfCirclesRadiusMillimeters);

            AddPointsToFigureAsSegment(topLeftInnerCircle);

            var verticalHalfCircleOffset = FirgureRelatedConstants.VERTICAL_DISTANCE_BETWEEN_HALF_CIRCLES_MILLIMETERS * pixelsPerMillimeter;

            var bottomLeftHalfCircleCenterPoint = new PointSingle(
                x: topLeftHalfCircleCenterPoint.X,
                y: topLeftHalfCircleCenterPoint.Y + verticalHalfCircleOffset);

            var bottomLeftHalfCircle = bottomLeftHalfCircleCenterPoint.GetCirclePoints(halfCirclesRadiusMillimeters,
                leftHalfCirclesStartAngleDegrees,
                leftHalfCirclesEndAngleDegrees);

            AddPointsToFigureAsSegment(bottomLeftHalfCircle);

            var bottomLeftInnerCirclePoint = new PointSingle(
                x: topLeftInnerCirclePoint.X,
                y: topLeftInnerCirclePoint.Y + verticalHalfCircleOffset);

            var bottomLeftInnerCircle = bottomLeftInnerCirclePoint.GetCirclePoints(innerHalfCirclesRadiusMillimeters);

            AddPointsToFigureAsSegment(bottomLeftInnerCircle);

            var topLeftHalfCircleTopLineFirstPoint = new PointSingle(
                x: topLeftHalfCircleCenterPoint.X,
                y: topLeftHalfCircleCenterPoint.Y - halfCirclesRadiusMillimeters);

            var topLeftHalfCircleTopLineSecondPoint = new PointSingle(
                 x: topLeftHalfCircleCenterPoint.X + distanceBetweenHalfCirclesAndLargeRectangleMillimeters,
                 y: topLeftHalfCircleCenterPoint.Y - halfCirclesRadiusMillimeters);

            AddPointsToFigureAsSegment([topLeftHalfCircleTopLineFirstPoint, topLeftHalfCircleTopLineSecondPoint]);

            var bottomLeftHalfCircleTopLineFirstPoint = new PointSingle(
                x: topLeftHalfCircleCenterPoint.X,
                y: topLeftHalfCircleCenterPoint.Y + halfCirclesRadiusMillimeters);

            var bottomLeftHalfCircleTopLineSecondPoint = new PointSingle(
                 x: topLeftHalfCircleCenterPoint.X + distanceBetweenHalfCirclesAndLargeRectangleMillimeters,
                 y: topLeftHalfCircleCenterPoint.Y + halfCirclesRadiusMillimeters);

            AddPointsToFigureAsSegment([bottomLeftHalfCircleTopLineFirstPoint, bottomLeftHalfCircleTopLineSecondPoint]);

            var bottomTopLeftHalfCircleTopLineFirstPoint = new PointSingle(
                x: bottomLeftHalfCircleCenterPoint.X,
                y: bottomLeftHalfCircleCenterPoint.Y - halfCirclesRadiusMillimeters);

            var bottomTopLeftHalfCircleTopLineSecondPoint = new PointSingle(
                 x: bottomLeftHalfCircleCenterPoint.X + distanceBetweenHalfCirclesAndLargeRectangleMillimeters,
                 y: bottomLeftHalfCircleCenterPoint.Y - halfCirclesRadiusMillimeters);

            AddPointsToFigureAsSegment([bottomTopLeftHalfCircleTopLineFirstPoint, bottomTopLeftHalfCircleTopLineSecondPoint]);

            var bottomBottomLeftHalfCircleTopLineFirstPoint = new PointSingle(
                x: bottomLeftHalfCircleCenterPoint.X,
                y: bottomLeftHalfCircleCenterPoint.Y + halfCirclesRadiusMillimeters);

            var bottomBottomLeftHalfCircleTopLineSecondPoint = new PointSingle(
                 x: bottomLeftHalfCircleCenterPoint.X + distanceBetweenHalfCirclesAndLargeRectangleMillimeters,
                 y: bottomLeftHalfCircleCenterPoint.Y + halfCirclesRadiusMillimeters);

            AddPointsToFigureAsSegment([bottomBottomLeftHalfCircleTopLineFirstPoint, bottomBottomLeftHalfCircleTopLineSecondPoint]);

            var rectangleWidthMillimeters = FirgureRelatedConstants.LARGE_RECTANGLE_WIDTH_MILLIMETERS * pixelsPerCentimeter;

            var topTopRightHalfCircleTopLineFirstPoint = new PointSingle(
                x: topLeftHalfCircleTopLineFirstPoint.X + rectangleWidthMillimeters,
                y: topLeftHalfCircleCenterPoint.Y - halfCirclesRadiusMillimeters);

            var topTopRightHalfCircleTopLineSecondPoint = new PointSingle(
                x: topTopRightHalfCircleTopLineFirstPoint.X + rectangleWidthMillimeters,
                y: topLeftHalfCircleCenterPoint.Y - halfCirclesRadiusMillimeters);

            AddPointsToFigureAsSegment([topTopRightHalfCircleTopLineFirstPoint, topTopRightHalfCircleTopLineSecondPoint]);
            /*
            var rectangleHeightMillimeters = topLeftHalfCircleTopLineSecondPoint.X - bottomLeftHalfCircleCenterPoint.X + distanceBetweenHalfCirclesAndLargeRectangleMillimeters;

            var rectangleTopLeft = topLeftHalfCircleTopLineSecondPoint;
            var rectangleBottomLeft = bottomBottomLeftHalfCircleTopLineSecondPoint;*/
        }

        private PointListTransformMessageParameter GetDrawingFigureMessage(IObjectTree parentMessage)
        {
            var rawBackgroundColor = _drawingSettingsProvider.Settings.BackgroundColor;
            var backgroundColor = new DrawingColor(rawBackgroundColor);

            var rawDrawingColor = _drawingSettingsProvider.Settings.DrawingColor;
            var drawingColor = new DrawingColor(rawDrawingColor);

            var thickness = _drawingSettingsProvider.Settings.FigureDrawingThickness;

            var parentFigureComponentDrawingMessage = new PointListTransformMessageParameter(
                points: [.. _figure.First().Points],
                color: drawingColor,
                transformMatrix: _transform,
                shouldFillGeometry: false,
                fillColor: default,
                clearBeforeRedraw: false,
                backgroundColor: backgroundColor,
                thickness: thickness,
                parent: parentMessage);

            foreach (var figureComponent in _figure.Skip(1))
            {
                var figureComponentDrawingMessage = parentFigureComponentDrawingMessage;

                parentFigureComponentDrawingMessage = new PointListTransformMessageParameter(
                    points: [.. figureComponent.Points],
                    color: drawingColor,
                    transformMatrix: _transform,
                    shouldFillGeometry: false,
                    fillColor: default,
                    clearBeforeRedraw: false,
                    backgroundColor: backgroundColor,
                    thickness: thickness,
                    parent: figureComponentDrawingMessage);
            }

            return parentFigureComponentDrawingMessage;
        }

        [RelayCommand]
        async Task InitializeCanvasAsync()
        {
            await LoadSettingsAsync();

            LoadCanvasState();

            InitializeGrid();

            InitializeAxis();
        }

        void InitializeGrid()
        {
            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;
            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            for (var y = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                y <= canvasSize.Height; y += (int)pixelsPerCentimeter)
            {
                for (var x = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                    x <= canvasSize.Width; x += (int)pixelsPerCentimeter)
                {
                    _grid.Add(new PointSingle(x, y));
                }

                if (y < canvasSize.Height)
                {
                    _grid.Add(DrawingConstants.INVALID_POINT);
                }
            }

            for (var x = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                x <= canvasSize.Width; x += (int)pixelsPerCentimeter)
            {
                for (var y = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                    y <= canvasSize.Height + pixelsPerCentimeter; y += (int)pixelsPerCentimeter)
                {
                    _grid.Add(new PointSingle(x, y));
                }

                if (x < canvasSize.Width)
                {
                    _grid.Add(DrawingConstants.INVALID_POINT);
                }
            }
        }

        void InitializeAxis()
        {
            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;
            var pixelsPerCentimeter = (int)_drawingSettingsProvider.Settings.PixelsPerCentimeter;
            var arrowHeadSize = DrawingConstants.X_Y_AXIS_TICKS_LENGTH_PIXELS;

            var centerX = canvasSize.Width / 2f;
            var centerY = canvasSize.Height / 2f;

            InitializeAxisLine(_horizontalAxis, canvasSize.Width, pixelsPerCentimeter, centerX, centerY, isVertical: false);
            AddArrowHead(_horizontalAxis, new PointSingle(canvasSize.Width, centerY), isVertical: false, arrowHeadSize);
            AddArrowHead(_horizontalAxis, new PointSingle(0, centerY), isVertical: false, arrowHeadSize, pointingLeft: true);

            InitializeAxisLine(_verticalAxis, canvasSize.Height, pixelsPerCentimeter, centerX, centerY, isVertical: true);
            AddArrowHead(_verticalAxis, new PointSingle(centerX, canvasSize.Height), isVertical: true, arrowHeadSize);
            AddArrowHead(_verticalAxis, new PointSingle(centerX, 0), isVertical: true, arrowHeadSize, pointingUp: true);
        }

        void AddArrowHead(List<PointSingle> axis, PointSingle endPoint, bool isVertical, float arrowHeadSize, bool pointingLeft = false, bool pointingUp = false)
        {
            axis.Add(DrawingConstants.INVALID_POINT);

            if (isVertical)
            {
                var tipX = endPoint.X;
                var tipY = endPoint.Y;

                axis.Add(new PointSingle(tipX, tipY));

                if (pointingUp)
                {
                    axis.Add(new PointSingle(tipX - arrowHeadSize / 2, tipY + arrowHeadSize));
                    axis.Add(new PointSingle(tipX, tipY));
                    axis.Add(new PointSingle(tipX + arrowHeadSize / 2, tipY + arrowHeadSize));
                }
                else
                {
                    axis.Add(new PointSingle(tipX - arrowHeadSize / 2, tipY - arrowHeadSize));
                    axis.Add(new PointSingle(tipX, tipY));
                    axis.Add(new PointSingle(tipX + arrowHeadSize / 2, tipY - arrowHeadSize));
                }

                axis.Add(new PointSingle(tipX, tipY));
            }
            else
            {
                var tipX = endPoint.X;
                var tipY = endPoint.Y;

                // Main arrow point (tip)
                axis.Add(new PointSingle(tipX, tipY));

                if (pointingLeft)
                {
                    // Horizontal arrow pointing left
                    axis.Add(new PointSingle(tipX + arrowHeadSize, tipY - arrowHeadSize / 2));
                    axis.Add(new PointSingle(tipX, tipY));
                    axis.Add(new PointSingle(tipX + arrowHeadSize, tipY + arrowHeadSize / 2));
                }
                else
                {
                    // Horizontal arrow pointing right
                    axis.Add(new PointSingle(tipX - arrowHeadSize, tipY - arrowHeadSize / 2));
                    axis.Add(new PointSingle(tipX, tipY));
                    axis.Add(new PointSingle(tipX - arrowHeadSize, tipY + arrowHeadSize / 2));
                }
                axis.Add(new PointSingle(tipX, tipY));
            }

            axis.Add(DrawingConstants.INVALID_POINT);
        }

        void InitializeMarksOnAxis()
        {
            var axisTickLength = _drawingSettingsProvider.Settings.AxisTickLength;
            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;
            var pixelsPerCentimeter = (int)_drawingSettingsProvider.Settings.PixelsPerCentimeter;
            var tickRange = (DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y, DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y + axisTickLength);

            var centerX = canvasSize.Width / 2f;
            var centerY = canvasSize.Height / 2f;

            InitializeAxisMarks(_horizontalAxisMarks,
                canvasSize.Width,
                pixelsPerCentimeter,
                tickRange,
                centerX,
                centerY - axisTickLength / 2,
                isVertical: false);

            InitializeAxisMarks(_verticalAxisMarks,
                canvasSize.Height,
                pixelsPerCentimeter,
                tickRange,
                centerX - axisTickLength / 2,
                centerY,
                isVertical: true);
        }

        void InitializeAxisLine(List<PointSingle> axis, int dimensionSize, int pixelsPerCentimeter, float centerX, float centerY, bool isVertical)
        {
            if (isVertical)
            {
                for (var position = centerY - pixelsPerCentimeter; position <= dimensionSize + pixelsPerCentimeter; position += pixelsPerCentimeter)
                {
                    axis.Add(new PointSingle(centerX, position));
                }
            }
            else
            {
                for (var position = centerX; position <= dimensionSize + pixelsPerCentimeter; position += pixelsPerCentimeter)
                {
                    axis.Add(new PointSingle(position, centerY));
                }
            }

            axis.Add(DrawingConstants.INVALID_POINT);

            if (isVertical)
            {
                for (var position = centerY - pixelsPerCentimeter; position >= -pixelsPerCentimeter; position -= pixelsPerCentimeter)
                {
                    axis.Add(new PointSingle(centerX, position));
                }
            }
            else
            {
                for (var position = centerX; position >= -pixelsPerCentimeter; position -= pixelsPerCentimeter)
                {
                    axis.Add(new PointSingle(position, centerY));
                }
            }

            axis.Add(DrawingConstants.INVALID_POINT);
        }

        void InitializeAxisMarks(List<PointSingle> axisMarks, int dimensionSize, int pixelsPerCentimeter, (float start, float end) tickRange, float centerX, float centerY, bool isVertical)
        {
            var (tickStart, tickEnd) = tickRange;

            if (isVertical)
            {
                for (var axisPosition = centerY; axisPosition < dimensionSize; axisPosition += pixelsPerCentimeter)
                {
                    for (var tickOffset = tickStart; tickOffset <= tickEnd; tickOffset++)
                    {
                        axisMarks.Add(new PointSingle(centerX + tickOffset, axisPosition));
                    }
                    axisMarks.Add(DrawingConstants.INVALID_POINT);
                }
            }
            else
            {
                for (var axisPosition = centerX; axisPosition < dimensionSize; axisPosition += pixelsPerCentimeter)
                {
                    for (var tickOffset = tickStart; tickOffset <= tickEnd; tickOffset++)
                    {
                        axisMarks.Add(new PointSingle(axisPosition, centerY + tickOffset));
                    }
                    axisMarks.Add(DrawingConstants.INVALID_POINT);
                }
            }

            axisMarks.Add(DrawingConstants.INVALID_POINT);

            if (isVertical)
            {
                for (var axisPosition = centerY - pixelsPerCentimeter; axisPosition >= 0; axisPosition -= pixelsPerCentimeter)
                {
                    for (var tickOffset = tickStart; tickOffset <= tickEnd; tickOffset++)
                    {
                        axisMarks.Add(new PointSingle(centerX + tickOffset, axisPosition));
                    }
                    axisMarks.Add(DrawingConstants.INVALID_POINT);
                }
            }
            else
            {
                for (var axisPosition = centerX - pixelsPerCentimeter; axisPosition >= 0; axisPosition -= pixelsPerCentimeter)
                {
                    for (var tickOffset = tickStart; tickOffset <= tickEnd; tickOffset++)
                    {
                        axisMarks.Add(new PointSingle(axisPosition, centerY + tickOffset));
                    }
                    axisMarks.Add(DrawingConstants.INVALID_POINT);
                }
            }

            axisMarks.Add(DrawingConstants.INVALID_POINT);
        }

        void InitializeCircle()
        {
            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;
            var pixelsPerCentimeter = (int)_drawingSettingsProvider.Settings.PixelsPerCentimeter;

            var centerPoint = new PointSingle(canvasSize.Width / 2f, canvasSize.Height / 2f);

            _userPoint.AddRange(centerPoint.GetCirclePoints(radius: 5));

            _userPoint.Add(DrawingConstants.INVALID_POINT);
        }

        void LoadCanvasState()
        {
            var backgroundColor = new DrawingColor(_drawingSettingsProvider.Settings.BackgroundColor);
            ClearCanvas(backgroundColor);
        }

        void InitializeCanvas()
        {
            WeakReferenceMessenger.Default.Send(new InitializeCanvasControlMessage(this));
        }

        void InitializeDrawingSession()
        {
            WeakReferenceMessenger.Default.Send(new InitializeDrawingSessionMessage(this));
        }

        void ClearCanvas(DrawingColor color)
        {
            var message = new ClearCanvasMessageParameter(color, clearBeforeRedraw: true);
            WeakReferenceMessenger.Default.Send(new ClearCanvasMessage(this, message));
        }

        void InitializeSettings()
        {
            WeakReferenceMessenger.Default.Send(new InitializeSettingsMessage(this));
        }

        async Task LoadSettingsAsync()
        {
            await WeakReferenceMessenger.Default.Send(new LoadSettingsAsyncMessage(this));
        }
    }
}
