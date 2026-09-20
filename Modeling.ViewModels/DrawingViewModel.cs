using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Modeling.ViewModels
{
    public sealed partial class DrawingViewModel : ObservableObject
    {
        readonly IDrawingSettingsProvider _drawingSettingsProvider;

        readonly List<PointSingle> _grid;
        readonly List<PointSingle> _horizontalAxis;
        readonly List<PointSingle> _verticalAxis;
        readonly List<PointSingle> _horizontalAxisMarks;
        readonly List<PointSingle> _verticalAxisMarks;
        readonly List<PointSingle> _userPoint;
        readonly List<PointSingle> _figure;

        PointSingle _previousMovedPoint;
        Matrix3x3Single _transform;

        PointListTransformMessageParameter _gridWithAxisDrawingMessage;

        public DrawingViewModel()
        {
            _figure = [];
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
        void CanvasPointerMoved(PointerRoutedEventArgs arguments)
        {
            try
            {
                var pointerPoint = arguments.GetCurrentPoint(default);
                var position = pointerPoint.Position;

                var newPoint = new PointSingle((float)position.X, (float)position.Y);

                var thickness = _drawingSettingsProvider.Settings.DrawingThickness;

                var rawBackgroundColor = _drawingSettingsProvider.Settings.BackgroundColor;
                var backgroundColor = new DrawingColor(rawBackgroundColor);

                var circleColor = _drawingSettingsProvider.Settings.VerticalAxisColor;
                var circleDrawingColor = new DrawingColor(circleColor);

                if (newPoint != _previousMovedPoint)
                {
                    var circleTransform = _transform * MatrixExtensions.CreateTranslationTransform(
                        _previousMovedPoint.X - newPoint.X,
                        _previousMovedPoint.Y - newPoint.Y);

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

                    _previousMovedPoint = newPoint;
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
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

            InitializeGrid();
            InitializeAxis();
            InitializeMarksOnAxis();
            InitializeCircle();

            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;

            var verticalCenterPoint = canvasSize.Height / 2;

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

            var thickness = _drawingSettingsProvider.Settings.DrawingThickness;
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

            WeakReferenceMessenger.Default.Send(new TransformPointsMessage(this, drawVerticalAxisMarksMessageParameter));

            _gridWithAxisDrawingMessage = drawVerticalAxisMarksMessageParameter;
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

            var centerX = canvasSize.Width / 2f;
            var centerY = canvasSize.Height / 2f;
            var radius = 10;

            const float anglaDegrees = 1f;
            const float angleRadians = anglaDegrees * (MathF.PI / 180f);
            const int numSegments = 360;

            for (var i = 0; i <= numSegments; i++)
            {
                var angle = i * 2 * MathF.PI / numSegments;
                var x = centerX + radius * MathF.Cos(angle);
                var y = centerY + radius * MathF.Sin(angle);

                _userPoint.Add(new PointSingle(x, y));
            }

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
