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
using Modeling.Core.Miscellaneous;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Extensions;
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
        static readonly PointSingle START_DRAWING_POINT = new PointSingle(769, 35);

        readonly IDrawingSettingsProvider _drawingSettingsProvider;

        readonly List<PointSingle> _grid;
        readonly List<PointSingle> _horizontalAxis;
        readonly List<PointSingle> _verticalAxis;
        readonly List<PointSingle> _horizontalAxisMarks;
        readonly List<PointSingle> _verticalAxisMarks;
        readonly List<PointSingle> _userPoint;
        readonly IFigure _figure;

        UserPointDrawingAction _drawingAction;
        PointSingle _previousMovedPoint;
        Matrix3x3Single _transform;

        PointListTransformMessageParameter _gridWithAxisDrawingMessage;

        public DrawingViewModel()
        {
            _drawingAction = UserPointDrawingAction.None;

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
        void CanvasPointerMoved(PointSingle point)
        {
            switch (_drawingAction)
            {
                case UserPointDrawingAction.AxisPointSelection: RedrawUserPoint(point); break;
                case UserPointDrawingAction.FigurePointSelection: break;
                default: break;
            }
        }

        void RedrawUserPoint(PointSingle point)
        {
            try
            {
                RedrawUserPointCore(point);
            }
            catch (Exception ex)
            {
                if (ex is not OperationCanceledException)
                {
                    Logger.Exception(ex);
                }
            }
        }

        void RedrawUserPointCore(PointSingle point)
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
        
        private void InitializeFigure()
        {
            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;
            var pixelsPerMillimeter = pixelsPerCentimeter / 10f;

            _figure.AddSegments(
                FigureExtensions.CreateCustomShape(START_DRAWING_POINT,
                pixelsPerCentimeter,
                FigureRelatedConstants.HALF_CIRCLES_DIAMETER_MILLIMETERS,
                FigureRelatedConstants.INNER_HALF_CIRCLES_DIAMETER_MILLIMETERS,
                FigureRelatedConstants.DISTANCE_BETWEEN_HALF_CIRCLES_AND_LARGE_RECTANGLE,
                FigureRelatedConstants.VERTICAL_DISTANCE_BETWEEN_HALF_CIRCLES_MILLIMETERS,
                FigureRelatedConstants.LARGE_RECTANGLE_WIDTH_MILLIMETERS,
                FigureRelatedConstants.SMALL_SQUARES_DIMENSION_SIZE_MILLIMETERS,
                FigureRelatedConstants.LARGE_CIRCLE_DIAMETER_MILLIMETERS,
                FigureRelatedConstants.SMALL_CIRCLE_DIAMETER_MILLIMETERS));
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

            _grid.AddRange(FigureExtensions.CreateGrid(canvasSize, pixelsPerCentimeter));
        }

        void InitializeAxis()
        {
            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;
            var pixelsPerCentimeter = (int)_drawingSettingsProvider.Settings.PixelsPerCentimeter;
            var arrowHeadSize = DrawingConstants.X_Y_AXIS_TICKS_LENGTH_PIXELS;

            var centerX = canvasSize.Width / 2f;
            var centerY = canvasSize.Height / 2f;

            _horizontalAxis.AddRange(FigureExtensions.CreateAxisLine(canvasSize.Width, pixelsPerCentimeter, centerX, centerY, isVertical: false));
            _horizontalAxis.AddRange(FigureExtensions.CreateArrowHead(new PointSingle(canvasSize.Width, centerY), isVertical: false, arrowHeadSize));
            _horizontalAxis.AddRange(FigureExtensions.CreateArrowHead(new PointSingle(0, centerY), isVertical: false, arrowHeadSize, pointingLeft: true));

            _verticalAxis.AddRange(FigureExtensions.CreateAxisLine(canvasSize.Height, pixelsPerCentimeter, centerX, centerY, isVertical: true));
            _verticalAxis.AddRange(FigureExtensions.CreateArrowHead(new PointSingle(centerX, canvasSize.Height), isVertical: true, arrowHeadSize));
            _verticalAxis.AddRange(FigureExtensions.CreateArrowHead(new PointSingle(centerX, 0), isVertical: true, arrowHeadSize, pointingUp: true));
        }

        void InitializeMarksOnAxis()
        {
            var axisTickLength = _drawingSettingsProvider.Settings.AxisTickLength;
            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;
            var pixelsPerCentimeter = (int)_drawingSettingsProvider.Settings.PixelsPerCentimeter;

            var centerX = canvasSize.Width / 2f;
            var centerY = canvasSize.Height / 2f;


            _horizontalAxisMarks.AddRange(FigureExtensions.CreateAxisMarks(
                canvasSize.Width,
                pixelsPerCentimeter,
                DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y,
                DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y + axisTickLength,
                centerX,
                centerY - axisTickLength / 2,
                isVertical: false));

            _verticalAxisMarks.AddRange(FigureExtensions.CreateAxisMarks(
                canvasSize.Height,
                pixelsPerCentimeter,
                DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y,
                DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y + axisTickLength,
                centerX - axisTickLength / 2,
                centerY,
                isVertical: true));
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
