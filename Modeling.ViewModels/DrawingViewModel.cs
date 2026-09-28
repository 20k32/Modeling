using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Modeling.Core.Abstractions;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Drawing.Providers;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Core.Messages.Canvas.Settings;
using Modeling.Core.Messages.Parameters.Canvas.Drawing;
using Modeling.Core.Messages.Parameters.Canvas.Settings;
using Modeling.Core.Messages.Settings;
using Modeling.Core.Messages.ViewModels;
using Modeling.Core.Navigation;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.Figures.PointGeometries;
using Modeling.Models.Extensions;
using Modeling.ViewModels.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Modeling.ViewModels
{
    public sealed partial class DrawingViewModel : ObservableObject
    {
        readonly IDrawingSettingsProvider _drawingSettingsProvider;
        readonly INavigationProvider _navigationProvider;

        readonly List<PointSingle> _grid;
        readonly List<PointSingle> _horizontalAxis;
        readonly List<PointSingle> _horizontalAxisArrows;
        readonly List<PointSingle> _verticalAxisArrows;
        readonly List<PointSingle> _verticalAxis;
        readonly List<PointSingle> _horizontalAxisMarks;
        readonly List<PointSingle> _verticalAxisMarks;
        readonly List<PointSingle> _userPoint;
        readonly IFigure _figure;

        PointSingle startDrawingPoint;

        UserPointDrawingAction _drawingAction;
        PointSingle _previousMovedPoint;
        Matrix3x3Single _transform;

        PointListTransformMessageParameter _lastDrawingMessage;

        [ObservableProperty]
        bool _pickShapeForResizing;

        [ObservableProperty]
        bool _changeFigurePosition;

        [ObservableProperty]
        bool _additionalPanelVisible;

        [ObservableProperty]
        bool _lineEditingPanelVisible;

        [ObservableProperty]
        bool _circleEditingPanelVisible;

        [ObservableProperty]
        IPointGeometry _nearestSegment;

        public DrawingViewModel()
        {
            _drawingAction = UserPointDrawingAction.None;

            _figure = Ioc.Default.GetRequiredService<IFigure>();
            _grid = [];

            _horizontalAxis = [];
            _horizontalAxisArrows = [];
            _horizontalAxisMarks = [];

            _verticalAxis = [];
            _verticalAxisArrows = [];
            _verticalAxisMarks = [];

            _userPoint = [];

            _transform = DrawingConstants.NON_TRANSFORM_MATRIX;

            _drawingSettingsProvider = Ioc.Default.GetRequiredService<IDrawingSettingsProvider>();
            _drawingSettingsProvider.SettingsChanged += OnDrawingSettingsProviderSettingsChanged;

            WeakReferenceMessenger.Default.Register<ChangeCanvasSettingsMessage>(this, OnChangeCanvasSettingsMessage);

            _navigationProvider = Ioc.Default.GetRequiredService<INavigationProvider>();
            _navigationProvider.Navigated += OnNavigationProviderNavigated;
        }

        partial void OnPickShapeForResizingChanged(bool value)
        {
            if (!value
                && _nearestSegment is not null
                && _figure.Any())
            {
                RedrawFigure();
                AdditionalPanelVisible = true;
            }
        }

        void OnNavigationProviderNavigated(NavigationPage page)
        {
            try
            {
                HandleSubPageNavigation(page);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        void HandleSubPageNavigation(NavigationPage page)
        {
            switch (page)
            {
                case (NavigationPage.ResizingPage): HandleResizingSubPageNavigation(); break;
                case (NavigationPage.AffineTransformPage): break;
                case (NavigationPage.EuclideanTransformPage): break;
                case (NavigationPage.ProjectiveTransformPage): break;
                default: break;
            }
        }

        void ResetStateForResizingSubPage()
        {
            PickShapeForResizing = true;
            _drawingAction = UserPointDrawingAction.FigurePointSelection;
        }

        void HandleResizingSubPageNavigation()
        {
            ResetStateForResizingSubPage();
        }

        void OnChangeCanvasSettingsMessage(object recipient, ChangeCanvasSettingsMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                if (message.Value.ShouldUpdateCanvasSize)
                {
                    var changeCanvasSizeMessage = new ChangeCanvasSizeMessage(this, message.Value);
                    WeakReferenceMessenger.Default.Send(changeCanvasSizeMessage);
                }

                if (message.Value.ShouldRecreateFigure)
                {
                    ClearDrawingElements();
                    InitializeDrawingElements(message.Value.Size, message.Value.PixelsPerCentimeter);
                }

                if (message.Value.ShouldUpdateRefreshRate)
                {
                    var newRefreshRate = TimeSpan.FromSeconds(1 / (double)message.Value.RefreshRate);
                    var refreshRateParameter = new RefreshRateParameter(newRefreshRate);
                    var changeRefreshRateMessage = new ChangeCanvasRefreshRateMessage(this, refreshRateParameter);
                    WeakReferenceMessenger.Default.Send(changeRefreshRateMessage);
                }
            }
        }

        void OnDrawingSettingsProviderSettingsChanged()
        {
            if (_figure.Any())
            {
                RedrawFigure();
            }
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
            _drawingAction = UserPointDrawingAction.AxisPointSelection;
            RedrawFigure();
        }

        [RelayCommand]
        void CanvasPointerMoved(PointSingle point)
        {
            switch (_drawingAction)
            {
                case UserPointDrawingAction.AxisPointSelection: RedrawUserPointCore(point); break;
                case UserPointDrawingAction.FigurePointSelection: SelectSegmentOnFigure(point); break;
                default: break;
            }
        }

        [RelayCommand]
        void CanvasPointerPressed(PointSingle point)
        {
            switch (_drawingAction)
            {
                case UserPointDrawingAction.AxisPointSelection: RedrawUserPointCore(point); break;
                case UserPointDrawingAction.FigurePointSelection: EndSegmentSelection(); break;
                default: break;
            }
        }

        void EndSegmentSelection()
        {
            PickShapeForResizing = false;
        }

        void SelectSegmentOnFigure(PointSingle point)
        {
            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            var shouldHandlePoint = PickShapeForResizing && _figure.Bounds.Contains(point, pixelsPerCentimeter);

            var nearestSegment = default(IPointGeometry);

            if (shouldHandlePoint)
            {
                nearestSegment = _figure.FindNearestSegment(point, pixelsPerCentimeter);
                shouldHandlePoint &= nearestSegment is not null;
            }

            Logger.Information($"X:{point.X} Y:{point.Y}");

            if (shouldHandlePoint && nearestSegment != NearestSegment)
            {
                NearestSegment = nearestSegment;

                var segmentRedrawingMessageParameter = _lastDrawingMessage.With(
                    points: [.. nearestSegment.Bounds.GetPointsFromBounds()],
                    color: new(_drawingSettingsProvider.Settings.FigureBoundsColor));

                WeakReferenceMessenger.Default.Send(new TransformPointsMessage(this, segmentRedrawingMessageParameter));
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
                    parent: _lastDrawingMessage);

                WeakReferenceMessenger.Default.Send(new TransformPointsMessage(this, drawCircleMessageParameter));

                _previousMovedPoint = point;

                Logger.Information($"X: {point.X} Y: ; {point.Y}");
            }
        }

        void RedrawFigure()
        {
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

            var axisThickness = _drawingSettingsProvider.Settings.AxisThickness;
            var axisTickThickness = _drawingSettingsProvider.Settings.AxisTickThickness;

            var thickness = _drawingSettingsProvider.Settings.GridDrawingThickness;

            var currentDrawingMessage = new PointListTransformMessageParameter(
                    points: [],
                    color: drawingColor,
                    transformMatrix: _transform,
                    clearBeforeRedraw: true,
                    backgroundColor: backgroundColor,
                    thickness: thickness);

            if (_drawingSettingsProvider.Settings.DrawGrid)
            {
                currentDrawingMessage = currentDrawingMessage.With(
                    points: _grid,
                    clearBeforeRedraw: false);
            }

            if (_drawingSettingsProvider.Settings.DrawAxis)
            {
                currentDrawingMessage = currentDrawingMessage
                .With(points: _horizontalAxis,
                      color: horizontalAxisDrawingColor,
                      clearBeforeRedraw: false,
                      thickness: axisThickness)
                .With(points: _verticalAxis,
                      color: verticalAxisDrawingColor);
            }

            if (_drawingSettingsProvider.Settings.DrawAxisArrows)
            {
                currentDrawingMessage = currentDrawingMessage
                .With(points: _horizontalAxisArrows,
                      color: horizontalAxisDrawingColor,
                      clearBeforeRedraw: false,
                      thickness: axisThickness)
                .With(points: _verticalAxisArrows,
                      color: verticalAxisDrawingColor);
            }

            if (_drawingSettingsProvider.Settings.DrawAxisMarks)
            {
                currentDrawingMessage = currentDrawingMessage
                 .With(points: _horizontalAxisMarks,
                      clearBeforeRedraw: false,
                      color: horizontalAxisTicksDrawingColor,
                      thickness: axisTickThickness)
                .With(points: _verticalAxisMarks);
            }


            currentDrawingMessage = GetDrawingFigureMessage(currentDrawingMessage);

            WeakReferenceMessenger.Default.Send(new TransformPointsMessage(this, currentDrawingMessage));

            _lastDrawingMessage = currentDrawingMessage;
        }

        void InitializeFigure(SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            var pixelsPerMillimeter = pixelsPerCentimeter / 10;

            var figureWidthPixels = FigureRelatedConstants.FIGURE_WIDTH_MILLIMETERS * pixelsPerMillimeter;
            var figureHeightPixels = FigureRelatedConstants.FIGURE_HEIGHT_MILLIMETERS * pixelsPerMillimeter;

            var halfCirclesRadiusPixels = FigureRelatedConstants.HALF_CIRCLES_DIAMETER_MILLIMETERS * pixelsPerMillimeter / 2;

            startDrawingPoint = new PointSingle(
                x: (canvasSize.Width / 2) - figureWidthPixels / 2 - halfCirclesRadiusPixels,
                y: (canvasSize.Height / 2) - figureHeightPixels / 2 - halfCirclesRadiusPixels);

            _figure.AddSegments(
                FigureExtensions.CreateCustomShape(startDrawingPoint,
                pixelsPerCentimeter,
                FigureRelatedConstants.HALF_CIRCLES_DIAMETER_MILLIMETERS,
                FigureRelatedConstants.INNER_HALF_CIRCLES_DIAMETER_MILLIMETERS,
                FigureRelatedConstants.DISTANCE_BETWEEN_HALF_CIRCLES_AND_LARGE_RECTANGLE,
                FigureRelatedConstants.VERTICAL_DISTANCE_BETWEEN_HALF_CIRCLES_MILLIMETERS,
                FigureRelatedConstants.LARGE_RECTANGLE_WIDTH_MILLIMETERS,
                FigureRelatedConstants.SMALL_SQUARES_DIMENSION_SIZE_MILLIMETERS,
                FigureRelatedConstants.LARGE_CIRCLE_DIAMETER_MILLIMETERS,
                FigureRelatedConstants.SMALL_CIRCLE_DIAMETER_MILLIMETERS));

            _figure.CalculatePropertiesFromSegments();
        }

        private PointListTransformMessageParameter GetDrawingFigureMessage(PointListTransformMessageParameter parentMessage)
        {
            var rawBackgroundColor = _drawingSettingsProvider.Settings.BackgroundColor;
            var backgroundColor = new DrawingColor(rawBackgroundColor);

            var rawDrawingColor = _drawingSettingsProvider.Settings.DrawingColor;
            var drawingColor = new DrawingColor(rawDrawingColor);

            var thickness = _drawingSettingsProvider.Settings.FigureDrawingThickness;

            var parentFigureComponentDrawingMessage = parentMessage
                .With(points: [.. _figure.First().Points],
                      color: drawingColor,
                      thickness: thickness);

            foreach (var figureComponent in _figure.Skip(1))
            {
                parentFigureComponentDrawingMessage =
                    parentFigureComponentDrawingMessage.With(points: [.. figureComponent.Points]);
            }

            _figure.CalculatePropertiesFromSegments();

            if (_drawingSettingsProvider.Settings.DrawFigureBounds)
            {
                parentFigureComponentDrawingMessage = parentFigureComponentDrawingMessage.With(
                points: [.. _figure.Bounds.GetPointsFromBounds()],
                color: new(_drawingSettingsProvider.Settings.FigureBoundsColor));
            }

            if (_drawingSettingsProvider.Settings.DrawFigureShapeBounds)
            {
                foreach (var segment in _figure.Segments)
                {
                    parentFigureComponentDrawingMessage =
                        parentFigureComponentDrawingMessage.With(
                            points: [.. segment.Bounds.GetPointsFromBounds()],
                            color: new(_drawingSettingsProvider.Settings.FigureBoundsColor));
                }
            }

            return parentFigureComponentDrawingMessage;
        }

        [RelayCommand]
        async Task InitializeCanvasAsync()
        {
            await LoadSettingsAsync();

            LoadCanvasState();

            RedrawFigure();
        }

        void InitializeGrid(SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            _grid.AddRange(FigureExtensions.CreateGrid(canvasSize, pixelsPerCentimeter));
        }

        void InitializeAxis(SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            var arrowHeadSize = DrawingConstants.X_Y_AXIS_TICKS_LENGTH_PIXELS;

            var centerX = canvasSize.Width / 2f;
            var centerY = canvasSize.Height / 2f;

            _horizontalAxis.AddRange(FigureExtensions.CreateAxisLine(canvasSize.Width, pixelsPerCentimeter, centerX, centerY, isVertical: false));
            _horizontalAxisArrows.AddRange(FigureExtensions.CreateArrowHead(new PointSingle(canvasSize.Width, centerY), isVertical: false, arrowHeadSize));
            _horizontalAxisArrows.AddRange(FigureExtensions.CreateArrowHead(new PointSingle(0, centerY), isVertical: false, arrowHeadSize, pointingLeft: true));

            _verticalAxis.AddRange(FigureExtensions.CreateAxisLine(canvasSize.Height, pixelsPerCentimeter, centerX, centerY, isVertical: true));
            _verticalAxisArrows.AddRange(FigureExtensions.CreateArrowHead(new PointSingle(centerX, canvasSize.Height), isVertical: true, arrowHeadSize));
            _verticalAxisArrows.AddRange(FigureExtensions.CreateArrowHead(new PointSingle(centerX, 0), isVertical: true, arrowHeadSize, pointingUp: true));
        }

        void InitializeMarksOnAxis(SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            var axisTickLength = _drawingSettingsProvider.Settings.AxisTickLength;

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

        void InitializeCircle(SizeSingle canvasSize)
        {
            var centerPoint = new PointSingle(canvasSize.Width / 2f, canvasSize.Height / 2f);

            _userPoint.AddRange(centerPoint.GetCirclePoints(radius: 5));

            _userPoint.Add(DrawingConstants.BREAK_POINT);
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

        void ClearDrawingElements()
        {
            _grid.Clear();
            _horizontalAxis.Clear();
            _horizontalAxisArrows.Clear();
            _verticalAxisArrows.Clear();
            _verticalAxis.Clear();
            _horizontalAxisMarks.Clear();
            _verticalAxisMarks.Clear();
            _userPoint.Clear();
            _figure.Segments.Clear();
        }

        void InitializeDrawingElements(SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            InitializeGrid(canvasSize, pixelsPerCentimeter);
            InitializeAxis(canvasSize, pixelsPerCentimeter);
            InitializeMarksOnAxis(canvasSize, pixelsPerCentimeter);
            InitializeCircle(canvasSize);
            InitializeFigure(canvasSize, pixelsPerCentimeter);
        }

        async Task LoadSettingsAsync()
        {
            await WeakReferenceMessenger.Default.Send(new LoadSettingsAsyncMessage(this));
        }
    }
}
