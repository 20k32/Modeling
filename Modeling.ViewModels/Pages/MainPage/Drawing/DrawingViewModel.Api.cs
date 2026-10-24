using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml.Media;
using Modeling.Core.Abstractions;
using Modeling.Core.Abstractions.Collections.Drawings;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Drawing.Providers;
using Modeling.Core.Enums;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Core.Messages.Canvas.Settings;
using Modeling.Core.Messages.Parameters.Canvas.Drawing;
using Modeling.Core.Messages.Parameters.Canvas.Settings;
using Modeling.Core.Messages.Settings;
using Modeling.Core.Messages.ViewModels;
using Modeling.Core.Miscellaneous;
using Modeling.Core.Navigation;
using Modeling.Models.Abstractions.Drawing;
using Modeling.Models.Abstractions.Drawing.Figure;
using Modeling.Models.Drawing.Figures;
using Modeling.Models.Drawing.Figures.PointGeometries;
using Modeling.Models.Drawing.Figures.PointGeometries.Abstractions;
using Modeling.Models.Drawing.Figures.PointGeometries.Enums;
using Modeling.Models.Drawing.Figures.PointGeometries.GeometryCreationFactory;
using Modeling.Models.Extensions;
using Modeling.Models.Miscellaneous;
using Modeling.Models.Navigation;
using Modeling.Models.UserInterface;
using Modeling.ViewModels.Miscellaneous;
using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.ViewModels.Pages.MainPage.Drawing
{
    public sealed partial class DrawingViewModel : BaseViewModel
    {
        readonly IDrawingSettingsProvider _drawingSettingsProvider;
        readonly INavigationProvider _navigationProvider;

        readonly IPointListCollection _grid;
        readonly IPointListCollection _horizontalAxis;
        readonly IPointListCollection _horizontalAxisArrows;
        readonly IPointListCollection _verticalAxisArrows;
        readonly IPointListCollection _verticalAxis;
        readonly IPointListCollection _horizontalAxisMarks;
        readonly IPointListCollection _verticalAxisMarks;
        readonly IPointGeometry _userPoint;
        readonly IFigure _figure;

        PointSingle _startDrawingPoint;

        UserPointDrawingAction _drawingAction;
        PointSingle _previousMovedPoint;

        Matrix3x3Single _figureTranslationTransform;
        Matrix3x3Single _figureProjectiveTransform;
        Matrix3x3Single _figureAffineTransform;
        Matrix3x3Single _figureRotationTransform;
        Matrix3x3Single _figureTransform;

        Matrix3x3Single _gridTransform;
        Matrix3x3Single _userPointTransform;

        IPointGeometry _pointerMoveNearestSegment;

        PointListTransformMessageParameter _lastDrawingMessage;

        bool _shouldChangeFigurePosition;
        bool _shouldApplyGeneralTransformForUserPoint;
        bool _canChangePositionForUserPoint;
        bool _canRedrawUserPoint;
        bool _wasRotationPointVisible;
        bool _wasPositionEditingControlVisible;

        float _lastRotationAngle;

        CancellationTokenSource _rotateAnimationCancellationSource;

        MouseCursor _cursorState;

        NavigationPage _currentLabPage;

        public DrawingViewModel()
        {
            _drawingAction = UserPointDrawingAction.None;

            _figure = Ioc.Default.GetRequiredService<IFigure>();
            _grid = Ioc.Default.GetRequiredService<IPointListCollection>();

            _horizontalAxis = Ioc.Default.GetRequiredService<IPointListCollection>();
            _horizontalAxisArrows = Ioc.Default.GetRequiredService<IPointListCollection>();
            _horizontalAxisMarks = Ioc.Default.GetRequiredService<IPointListCollection>();

            _verticalAxis = Ioc.Default.GetRequiredService<IPointListCollection>();
            _verticalAxisArrows = Ioc.Default.GetRequiredService<IPointListCollection>();
            _verticalAxisMarks = Ioc.Default.GetRequiredService<IPointListCollection>();

            _gridTransform = DrawingConstants.NON_TRANSFORM_MATRIX;

            _userPointTransform = DrawingConstants.NON_TRANSFORM_MATRIX;

            _figureRotationTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _figureTranslationTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _figureProjectiveTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _figureAffineTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _figureTransform = DrawingConstants.NON_TRANSFORM_MATRIX;

            _drawingSettingsProvider = Ioc.Default.GetRequiredService<IDrawingSettingsProvider>();
            _drawingSettingsProvider.SettingsChanged += OnDrawingSettingsProviderSettingsChanged;

            WeakReferenceMessenger.Default.Register<ChangeCanvasSettingsMessage>(this, OnChangeCanvasSettingsMessage);

            _navigationProvider = Ioc.Default.GetRequiredService<INavigationProvider>();
            _navigationProvider.Navigated += OnNavigationProviderNavigatedAsync;

            _pickButtonsVisible = true;

            _userPoint = Ioc.Default.GetRequiredService<IPointGeometryCreationFactory>()
                .CreateCirclePointGeometry(DrawingConstants.DEFAULT_POINT, SegmentDimensionParameter.None);

            ResetAffineTransforms();
            ResetProjectiveTransforms();
        }

        void SetDimensionLengthCentimetersSilent(float newValuePixels)
        {
            if (_dimensionLengthCentimeters != newValuePixels)
            {
                var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

                _dimensionLengthCentimeters = newValuePixels / pixelsPerCentimeter;

                OnPropertyChanged(nameof(DimensionLengthCentimeters));
            }
        }

        void HandleDistanceDisplaying(IPointGeometry pointGeometry)
        {
            switch (pointGeometry.GeometryType)
            {
                case GeometryType.Line: HandleLineDistanceDisplaying(pointGeometry); break;
                case GeometryType.Circle: HandleCircleDistanceDisplaying(pointGeometry); break;
                default: break;
            }
        }

        void HandleCircleDistanceDisplaying(IPointGeometry pointGeometry)
        {
            if (pointGeometry is ICirclePointGeometry circlePointGeometry)
            {
                SetDimensionLengthCentimetersSilent(circlePointGeometry.Diameter / 2);
            }
        }

        void HandleLineDistanceDisplaying(IPointGeometry pointGeometry)
        {
            if (pointGeometry is ILinePointGeometry linePointGeometry)
            {
                SetDimensionLengthCentimetersSilent(linePointGeometry.Length);
            }
        }

        async Task OnNavigationProviderNavigatedAsync(NavigationParameter navigationParameter)
        {
            if (navigationParameter is not null)
            {
                if (navigationParameter.ToPage != NavigationPage.None && navigationParameter.ToPage != NavigationPage.SettingsPage)
                {
                    _currentLabPage = navigationParameter.ToPage;
                }

                try
                {
                    await HandleSubPageNavigationAsync(navigationParameter);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }
        }

        async Task HandleSubPageNavigationAsync(NavigationParameter navigationParameter)
        {
            if (navigationParameter.ToPage != NavigationPage.None)
            {
                if (navigationParameter.ToPage != NavigationPage.SettingsPage)
                {
                    if (NavigationPage.FirstLabPage.HasFlag(navigationParameter.ToPage)
                        && !NavigationPage.FirstLabPage.HasFlag(navigationParameter.FromPage)
                        || NavigationPage.SecondLabPage.HasFlag(navigationParameter.ToPage)
                        && !NavigationPage.SecondLabPage.HasFlag(navigationParameter.FromPage))
                    {
                        await ResetFigureAsync();
                    }
                }
            }
        }

        void OnChangeCanvasSettingsMessage(object recipient, ChangeCanvasSettingsMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                message.Reply(ChangeCanvasSettingsCoreAsync(message));
            }
        }

        async Task<Unit> ChangeCanvasSettingsCoreAsync(ChangeCanvasSettingsMessage message)
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

            if (message.Value.ShouldApplyAntiAliasing)
            {
                await RedrawAllAsync();
            }

            return Unit.Default;
        }

        async void OnDrawingSettingsProviderSettingsChanged()
        {
            await RedrawAllAsync();
        }

        void EndSegmentSelection()
        {
            NearestSegment = _pointerMoveNearestSegment;
        }

        void SelectSegmentOnFigure(PointSingle point)
        {
            var inverted = _figureTransform.Inverse();

            point = _figureTransform.Inverse() * point;

            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            var shouldHandlePoint = PickShapeForResizing && _figure.Bounds.Contains(point, pixelsPerCentimeter);

            var nearestSegment = default(IPointGeometry);

            if (shouldHandlePoint)
            {
                nearestSegment = _figure.FindNearestSegment(point, pixelsPerCentimeter);
                shouldHandlePoint &= nearestSegment is not null;
            }

            Logger.Information($"X:{point.X} Y:{point.Y}");

            if (shouldHandlePoint && nearestSegment != _pointerMoveNearestSegment)
            {
                _pointerMoveNearestSegment = nearestSegment;

                var segmentRedrawingMessageParameter = _lastDrawingMessage.With(
                    points: nearestSegment.Bounds.GetPointsFromBounds(),
                    transformMatrix: _figureTransform,
                    thickness: _drawingSettingsProvider.Settings.FigureDrawingThickness,
                    color: new(_drawingSettingsProvider.Settings.FigureBoundsColor));

                WeakReferenceMessenger.Default.Send(new TransformPointsMessage(this, segmentRedrawingMessageParameter));
            }
        }

        void StartUserPointSelection(PointSingle point)
        {
            if (_canChangePositionForUserPoint)
            {
                return;
            }

            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            if (!_shouldApplyGeneralTransformForUserPoint)
            {
                point = _userPointTransform.Inverse() * point;
            }

            if (_userPoint.Bounds.Contains(point, pixelsPerCentimeter))
            {
                _canChangePositionForUserPoint = true;

                ChangeMouseCursor(MouseCursor.Move);
            }
        }

        void ChangeMouseCursorForUserPoint(IBounds boundsEntity, PointSingle point)
        {
            if (!_shouldApplyGeneralTransformForUserPoint)
            {
                point = _userPointTransform.Inverse() * point;
            }

            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            if (_canRedrawUserPoint)
            {
                if (boundsEntity.Bounds.Contains(point, pixelsPerCentimeter))
                {
                    ChangeMouseCursor(MouseCursor.Finger);
                }
                else if (_cursorState != MouseCursor.Default)
                {
                    ChangeMouseCursor(MouseCursor.Default);
                }
            }
        }

        void ChangeMouseCursorForEntireFigurePoint(IBounds boundsEntity, PointSingle point)
        {
            point = _figureTransform.Inverse() * point;

            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            if (boundsEntity.Bounds.Contains(point, pixelsPerCentimeter))
            {
                ChangeMouseCursor(MouseCursor.Finger);
            }
            else
            {
                ChangeMouseCursor(MouseCursor.Default);
            }
        }

        async Task RedrawUserPointCoreAsync(PointSingle point)
        {
            if (!_canChangePositionForUserPoint)
            {
                ChangeMouseCursorForUserPoint(_userPoint, point);
                return;
            }

            if (point != _previousMovedPoint)
            {
                await ApplyUserPointTranslationTransformAsync(point);

                SetHorizontalRotationPointCenterSilent(point.X);
                SetVerticalRotationPointCenterSilent(point.Y);
            }
        }

        async Task RedrawAllAsync()
        {
            var applyAliasingToDrawings = _drawingSettingsProvider.Settings.ApplyAntiAliasingToDrawings;
            var applyAliasingToGrid = _drawingSettingsProvider.Settings.ApplyAntiAliasingToGrid;

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
                    points: Ioc.Default.GetRequiredService<IPointListCollection>(),
                    color: drawingColor,
                    transformMatrix: _drawingSettingsProvider.Settings.AttachGridToFigure ? _figureTransform : _gridTransform,
                    clearBeforeRedraw: true,
                    backgroundColor: backgroundColor,
                    thickness: thickness);

            if (_drawingSettingsProvider.Settings.DrawGrid)
            {
                currentDrawingMessage = currentDrawingMessage.With(
                    points: _grid,
                    applyAliasing: applyAliasingToGrid,
                    drawingAliasing: applyAliasingToGrid ? DrawingAliasing.Aliased : DrawingAliasing.AntiAliased);
            }

            currentDrawingMessage = currentDrawingMessage.With(
                points: Ioc.Default.GetRequiredService<IPointListCollection>(),
                clearBeforeRedraw: false,
                applyAliasing: applyAliasingToDrawings,
                drawingAliasing: applyAliasingToDrawings ? DrawingAliasing.Aliased : DrawingAliasing.AntiAliased);

            if (_drawingSettingsProvider.Settings.DrawAxis)
            {
                currentDrawingMessage = currentDrawingMessage
                .With(points: _horizontalAxis,
                      color: horizontalAxisDrawingColor,
                      thickness: axisThickness)
                .With(points: _verticalAxis,
                      color: verticalAxisDrawingColor);
            }

            if (_drawingSettingsProvider.Settings.DrawAxisArrows)
            {
                currentDrawingMessage = currentDrawingMessage
                .With(points: _horizontalAxisArrows,
                      color: horizontalAxisDrawingColor,
                      thickness: axisThickness)
                .With(points: _verticalAxisArrows,
                      color: verticalAxisDrawingColor);
            }

            if (_drawingSettingsProvider.Settings.DrawAxisMarks)
            {
                currentDrawingMessage = currentDrawingMessage
                 .With(points: _horizontalAxisMarks,
                      color: horizontalAxisTicksDrawingColor,
                      thickness: axisTickThickness)
                .With(points: _verticalAxisMarks,
                      color: verticalAxisTicksDrawingColor);
            }

            if (_figure.Any())
            {
                currentDrawingMessage = GetDrawingFigureMessage(currentDrawingMessage);
            }

            if (_isCurveInitialized)
            {
                currentDrawingMessage = GetDrawingCurveMessage(currentDrawingMessage);
            }

            if (_canRedrawUserPoint)
            {
                var circleColor = _drawingSettingsProvider.Settings.VerticalAxisColor;
                var circleDrawingColor = new DrawingColor(circleColor);

                currentDrawingMessage = currentDrawingMessage.With(
                     points: _userPoint.Points,
                     color: circleDrawingColor,
                     transformMatrix: _shouldApplyGeneralTransformForUserPoint
                     ? _gridTransform
                     : _userPointTransform,
                     shouldFillGeometry: true,
                     fillColor: new DrawingColor(_drawingSettingsProvider.Settings.HorizontalAxisColor),
                     backgroundColor: backgroundColor,
                     thickness: thickness);

                if (_drawingSettingsProvider.Settings.DrawFigureShapeBounds)
                {
                    currentDrawingMessage = currentDrawingMessage.With(
                        points: _userPoint.Bounds.GetPointsFromBounds(),
                        shouldFillGeometry: false,
                        color: new(_drawingSettingsProvider.Settings.FigureBoundsColor),
                        thickness: _drawingSettingsProvider.Settings.FigureDrawingThickness);
                }
            }

            await WeakReferenceMessenger.Default.Send(new TransformPointsMessage(this, currentDrawingMessage));

            _lastDrawingMessage = currentDrawingMessage;
        }

        void InitializeFigureAccordingPage(NavigationPage page, SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            if (NavigationPage.FirstLabPage.HasFlag(page))
            {
                InitializeFirstLabFigure(canvasSize, pixelsPerCentimeter);
            }
            else if (NavigationPage.SecondLabPage.HasFlag(page))
            {
                InitializeSecondLabFigure(canvasSize, pixelsPerCentimeter);
            }
        }

        void InitializeFirstLabFigure(SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            var pixelsPerMillimeter = pixelsPerCentimeter / 10;

            var figureWidthPixels = FigureRelatedConstants.FIGURE_WIDTH_MILLIMETERS * pixelsPerMillimeter;
            var figureHeightPixels = FigureRelatedConstants.FIGURE_HEIGHT_MILLIMETERS * pixelsPerMillimeter;

            var halfCirclesRadiusPixels = FigureRelatedConstants.HALF_CIRCLES_DIAMETER_MILLIMETERS * pixelsPerMillimeter;

            _startDrawingPoint = new PointSingle(
                x: (canvasSize.Width - figureWidthPixels) / 2,
                y: (canvasSize.Height - figureHeightPixels) / 2);

            _figure.InitializeSegmentDimensions(pixelsPerCentimeter);

            InitializeFigureSegments(pixelsPerCentimeter);

            _figure.CalculateDefaultPropertiesFromSegments();

            SetHorizontalFigurePositionSilent(_figure.CenterPoint.X);
            SetVerticalFigurePositionSilent(_figure.CenterPoint.Y);
        }

        void InitializeSecondLabFigure(SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            InitializeCurve(canvasSize, pixelsPerCentimeter);
        }

        void InitializeFigureSegments(float pixelsPerCentimeter)
        {
            _figure.AddSegments(
                FigureExtensions.CreateCustomShape(_startDrawingPoint,
                _figure.GetActualSegmentsDimensions()),
                pixelsPerCentimeter);

            _figure.CalculatePropertiesFromSegments();
        }

        PointListTransformMessageParameter GetDrawingFigureMessage(PointListTransformMessageParameter parentMessage)
        {
            var rawBackgroundColor = _drawingSettingsProvider.Settings.BackgroundColor;
            var backgroundColor = new DrawingColor(rawBackgroundColor);

            var rawDrawingColor = _drawingSettingsProvider.Settings.DrawingColor;
            var drawingColor = new DrawingColor(rawDrawingColor);

            var thickness = _drawingSettingsProvider.Settings.FigureDrawingThickness;

            var parentFigureComponentDrawingMessage = parentMessage
                .With(points: _figure.First().Points,
                      color: drawingColor,
                      transformMatrix: _figureTransform,
                      thickness: thickness);

            foreach (var figureComponent in _figure.Skip(1))
            {
                parentFigureComponentDrawingMessage =
                    parentFigureComponentDrawingMessage.With(points: figureComponent.Points);
            }

            if (_drawingSettingsProvider.Settings.DrawFigureBounds)
            {
                parentFigureComponentDrawingMessage = parentFigureComponentDrawingMessage.With(
                points: _figure.Bounds.GetPointsFromBounds(),
                color: new(_drawingSettingsProvider.Settings.FigureBoundsColor));
            }

            if (_drawingSettingsProvider.Settings.DrawFigureShapeBounds)
            {
                foreach (var segment in _figure.Segments)
                {
                    parentFigureComponentDrawingMessage =
                        parentFigureComponentDrawingMessage.With(
                            points: segment.Bounds.GetPointsFromBounds(),
                            color: new(_drawingSettingsProvider.Settings.FigureBoundsColor));
                }
            }

            return parentFigureComponentDrawingMessage;
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

        void InitializeUserPoint(PointSingle centerPoint)
        {
            _canRedrawUserPoint = false;
            _canChangePositionForUserPoint = false;
            _shouldApplyGeneralTransformForUserPoint = true;

            var userPointRadius = 10;

            if (_userPoint.GeometryType == GeometryType.Circle && _userPoint is ICirclePointGeometry circleGeometry)
            {
                circleGeometry.CenterCirclePoint = centerPoint;
                circleGeometry.Diameter = userPointRadius;
            }

            _userPoint.Points.AddRange(centerPoint.GetCirclePoints(userPointRadius));
            _userPoint.CalculateBounds();
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
            var message = new ClearCanvasMessageParameter(color,
                clearBeforeRedraw: true);
            WeakReferenceMessenger.Default.Send(new ClearCanvasMessage(this, message));
        }

        void InitializeSettings()
        {
            WeakReferenceMessenger.Default.Send(new InitializeSettingsMessage(this));
        }

        void ClearDrawingElements()
        {
            _grid.Clear();

            _horizontalAxisArrows.Clear();
            _verticalAxisArrows.Clear();

            _horizontalAxis.Clear();
            _verticalAxis.Clear();

            _horizontalAxisMarks.Clear();
            _verticalAxisMarks.Clear();

            _figure.Segments.Clear();

            _userPoint.Points.Clear();

            ClearCurve();
        }

        void InitializeDrawingElements(SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            InitializeGrid(canvasSize, pixelsPerCentimeter);
            InitializeAxis(canvasSize, pixelsPerCentimeter);
            InitializeMarksOnAxis(canvasSize, pixelsPerCentimeter);

            InitializeFigureAccordingPage(_currentLabPage, canvasSize, pixelsPerCentimeter);

            var centerPoint = new PointSingle(canvasSize.Width / 2f, canvasSize.Height / 2f);
            InitializeUserPoint(centerPoint);

            SetHorizontalRotationPointCenterSilent(centerPoint.X);
            SetVerticalRotationPointCenterSilent(centerPoint.Y);
        }

        async Task LoadSettingsAsync()
        {
            await WeakReferenceMessenger.Default.Send(new LoadSettingsAsyncMessage(this));
        }

        void HandleDimensionLengthCentimetersChanged(float value)
        {
            switch (NearestSegment.GeometryType)
            {
                case GeometryType.Line: HandleLineLengthChanged(value); break;
                case GeometryType.Circle: HandleCircleLengthChanged(value); break;
                default: break;
            }
        }

        async Task HandleCircleLengthChanged(float value)
        {
            if (NearestSegment is not ICirclePointGeometry || value < 0)
            {
                return;
            }

            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            var newLength = value * 2 * pixelsPerCentimeter;

            await HandleSegmentSizeChangedAsync(NearestSegment, pixelsPerCentimeter, newLength);
        }

        async Task HandleLineLengthChanged(float value)
        {
            if (NearestSegment is not ILinePointGeometry || value < 0)
            {
                return;
            }

            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            var newLength = value * pixelsPerCentimeter;

            await HandleSegmentSizeChangedAsync(NearestSegment, pixelsPerCentimeter, newLength);
        }

        async Task HandleSegmentSizeChangedAsync(IPointGeometry segment, float pixelsPerCentimeter, float value)
        {
            _figure.Segments.Clear();

            _figure.ChangeSegmentDimension(NearestSegment.SegmentDimension, value);

            InitializeFigureSegments(pixelsPerCentimeter);

            await RedrawAllAsync();
        }

        async void OnNearestSegmentDimensionChanged()
        {
            await RedrawAllAsync();
        }

        void EndUserPointRedrawing(PointSingle point)
        {
            _canChangePositionForUserPoint = false;
            ChangeMouseCursor(MouseCursor.Default);
        }

        void ChangeMouseCursor(MouseCursor newState)
        {
            if (_cursorState != newState)
            {
                var changeCursorParameter = new CanvasCursorMessageParameter(newState);
                WeakReferenceMessenger.Default.Send(new ChangeCanvasCursorMessage(this, changeCursorParameter));
            }
        }

        void StartFigureSelection(PointSingle point)
        {
            if (_shouldChangeFigurePosition)
            {
                return;
            }

            var localPoint = _figureTransform.Inverse() * point;
            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            if (_figure.Bounds.Contains(localPoint, pixelsPerCentimeter))
            {
                _shouldChangeFigurePosition = true;
                _previousMovedPoint = point;

                ChangeMouseCursor(MouseCursor.Move);
            }
        }

        async Task SelectEntireFigureAsync(PointSingle point)
        {
            if (!_shouldChangeFigurePosition)
            {
                ChangeMouseCursorForEntireFigurePoint(_figure, point);
            }
            else if (point != _previousMovedPoint)
            {
                await ApplyFigureTranslationTransformAsync(point);

                var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;

                var centerPointTransformed = _figureTransform * new PointSingle(canvasSize.Width / 2, canvasSize.Height / 2);

                SetHorizontalFigurePositionSilent(centerPointTransformed.X);
                SetVerticalFigurePositionSilent(centerPointTransformed.Y);
            }
        }

        void EndFigureSelection(PointSingle point)
        {
            _shouldChangeFigurePosition = false;

            ChangeMouseCursor(MouseCursor.Default);
        }

        async Task RotateFigureAsync(float angleDegrees)
        {
            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;
            var pixelsPerMillimeter = pixelsPerCentimeter / 10;

            var rotationAngle = angleDegrees - _lastRotationAngle;
            _lastRotationAngle = angleDegrees;

            var transformedUserPoint = _userPointTransform * _userPoint.CenterPoint;

            var rotationDeltaTransform = MatrixExtensions.CreateRotationTransform(
                transformedUserPoint,
                -rotationAngle.DegreesToRadian());

            _figureRotationTransform = rotationDeltaTransform * _figureRotationTransform;
            RebuildFigureTransform();

            await RedrawAllAsync();
        }

        async Task StartAnimatingRotationAsync()
        {
            using (var animationCancellationSource = new CancellationTokenSource())
            {
                try
                {
                    var token = animationCancellationSource.Token;

                    _rotateAnimationCancellationSource = animationCancellationSource;

                    await AnimateRotationAsync(token);
                }
                catch (Exception ex)
                {
                    if (ex is not OperationCanceledException)
                    {
                        Logger.Exception(ex);
                    }
                }
            }
        }

        async Task AnimateRotationAsync(CancellationToken token)
        {
            var userInterfaceConstants = Ioc.Default.GetRequiredService<IUserInterfaceConstantsProvider>();

            var stepFrequency = userInterfaceConstants.SlidersStepFrequency;
            var animationTimeoutMilliseconds = userInterfaceConstants.AnimationTimeoutMilliseconds;

            var rotationAngle = _lastRotationAngle;

            var needToIncreaseAngle = rotationAngle + 1 < DrawingConstants.CIRCLE_END_ANGLE_DEGREES;
            var needToDecreaseAngle = !needToIncreaseAngle;

            while (!token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();

                SetRotationAngleSilent(rotationAngle);

                await RotateFigureAsync(rotationAngle);

                UpdateFigureCenterInUserInterface();

                if (needToDecreaseAngle)
                {
                    needToDecreaseAngle = rotationAngle - stepFrequency >= DrawingConstants.CIRCLE_START_ANGLE_DEGREES;

                    if (needToDecreaseAngle)
                    {
                        rotationAngle -= stepFrequency;
                    }
                    else
                    {
                        needToDecreaseAngle = false;
                        needToIncreaseAngle = true;
                    }
                }
                else if (needToIncreaseAngle)
                {
                    needToIncreaseAngle = rotationAngle + stepFrequency <= DrawingConstants.CIRCLE_END_ANGLE_DEGREES;

                    if (needToIncreaseAngle)
                    {
                        rotationAngle += stepFrequency;
                    }
                    else
                    {
                        needToIncreaseAngle = false;
                        needToDecreaseAngle = true;
                    }
                }

                await Task.Delay(animationTimeoutMilliseconds, token);
            }
        }

        private void UpdateFigureCenterInUserInterface()
        {
            var transformedFigureCenter = _figureTransform * _figure.CenterPoint;

            SetHorizontalFigurePositionSilent(transformedFigureCenter.X);
            SetVerticalFigurePositionSilent(transformedFigureCenter.Y);
        }

        async Task StopAnimatingRotationAsync()
        {
            try
            {
                await _rotateAnimationCancellationSource.CancelAsync();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        async Task ApplyAffineTransformAsync()
        {
            var origin = new PointSingle(AffineStartPointX, AffineStartPointY);

            var xAxis = new PointSingle(AffineNewXPointX, AffineNewXPointY);

            var yAxis = new PointSingle(AffineNewYPointX, AffineNewYPointY);

            var affineTransformMatrix = MatrixExtensions.CreateAffineTransform(origin, xAxis, yAxis);

            _figureAffineTransform = affineTransformMatrix;
            RebuildFigureTransform();

            await RedrawAllAsync();
        }

        async Task ApplyAffineTransformWithDelayAsync()
        {
            await WaitBeforeExecutionAsync(ApplyAffineTransformAsync);
        }

        async Task ApplyFigureTranslationTransformAsync(PointSingle point)
        {
            var offset = point - _previousMovedPoint;

            await MoveFigureToOffsetAsync(offset);

            _previousMovedPoint = point;
            _shouldApplyGeneralTransformForUserPoint = false;
        }

        async Task ApplyFigureTranslationTransformAsync2(PointSingle point)
        {
            var currentCenter = _figureTransform * _figure.CenterPoint;
            var offset = point - currentCenter;

            await MoveFigureToOffsetAsync(offset);
        }

        async Task MoveFigureToOffsetAsync(PointSingle offsetPoint)
        {
            var translationDelta = MatrixExtensions.CreateTranslationTransform(
                offsetPoint.X,
                offsetPoint.Y);

            _figureTranslationTransform = translationDelta * _figureTranslationTransform;
            RebuildFigureTransform();

            await RedrawAllAsync();
        }

        void RebuildFigureTransform()
        {
            _figureTransform =
                _figureTranslationTransform
                * _figureRotationTransform
                * _figureProjectiveTransform
                * _figureAffineTransform;
        }

        async Task ApplyFigureTranslationTransformWithDelayAsync(PointSingle point)
        {
            await WaitBeforeExecutionAsync(ApplyFigureTranslationTransformAsync2, point);
        }

        async Task ApplyUserPointTranslationTransformAsync(PointSingle point)
        {
            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;

            var centerX = canvasSize.Width / 2f;
            var centerY = canvasSize.Height / 2f;

            var offsetX = point.X - centerX;
            var offsetY = point.Y - centerY;

            _userPointTransform = MatrixExtensions.CreateTranslationTransform(
                offsetX,
                offsetY);

            _previousMovedPoint = point;

            Logger.Information($"X: {point.X} Y: ; {point.Y}");

            _shouldApplyGeneralTransformForUserPoint = false;

            await RedrawAllAsync();
        }

        async Task ApplyUserPointTranslationTransformWithDelayAsync(PointSingle point)
        {
            await WaitBeforeExecutionAsync(ApplyUserPointTranslationTransformAsync, point);
        }

        async Task CancelApiLayerAsync()
        {
            await RedrawAllAsync();
        }

        async Task ResetApiLayerAsync()
        {
            _lastRotationAngle = default;

            _shouldApplyGeneralTransformForUserPoint = true;

            _canRedrawUserPoint = false;

            _previousMovedPoint = DrawingConstants.BREAK_POINT;

            _shouldChangeFigurePosition = false;
            _canRedrawUserPoint = false;

            _wasPositionEditingControlVisible = false;
            _wasRotationPointVisible = false;

            _drawingAction = UserPointDrawingAction.None;

            _userPointTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _figureRotationTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _figureTranslationTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _figureProjectiveTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _figureAffineTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _figureTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _gridTransform = DrawingConstants.NON_TRANSFORM_MATRIX;

            ResetAffineTransforms();
            ResetProjectiveTransforms();

            await RedrawAllAsync();
        }

        void ResetAffineTransforms()
        {
            SetAffineStartPointXSilent(DrawingConstants.DEFAULT_COORDINATE_SYSTEM_START_POINT.X);
            SetAffineStartPointYSilent(DrawingConstants.DEFAULT_COORDINATE_SYSTEM_START_POINT.Y);
            SetAffineNewXPointXSilent(DrawingConstants.DEFAULT_X_AXIS_VECTOR.X);
            SetAffineNewXPointYSilent(DrawingConstants.DEFAULT_X_AXIS_VECTOR.Y);
            SetAffineNewYPointXSilent(DrawingConstants.DEFAULT_Y_AXIS_VECTOR.X);
            SetAffineNewYPointYSilent(DrawingConstants.DEFAULT_Y_AXIS_VECTOR.Y);
        }

        void ResetProjectiveTransforms()
        {
            SetProjectiveStartPointXSilent(0);
            SetProjectiveStartPointYSilent(0);

            SetProjectiveNewXPointXSilent(1);
            SetProjectiveNewXPointYSilent(0);

            SetProjectiveNewYPointXSilent(0);
            SetProjectiveNewYPointYSilent(1);

            SetProjectiveStartPointWeightSilent(1);
            SetProjectiveNewXWeightSilent(1);
            SetProjectiveNewYWeightSilent(1);
        }

        async Task ApplySymmetryTransformRelativeRotationPointAsync()
        {

            var rotationPoint = _userPointTransform * _userPoint.CenterPoint;

            var symmetryTransform = MatrixExtensions.CreatePointSymmetryTransform(rotationPoint);

            _figureTransform = symmetryTransform * _figureTransform;

            UpdateFigureCenterInUserInterface();

            await RedrawAllAsync();
        }

        async Task ApplyProjectiveTransformAsync()
        {
            if (_drawingSettingsProvider?.Settings is not null)
            {
                var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;

                var projectiveTransform = MatrixExtensions.CreateProjectiveTransform(
                       new PointSingle(ProjectiveStartPointX, ProjectiveStartPointY),
                       new PointSingle(ProjectiveNewXPointX * canvasSize.Width, ProjectiveNewXPointY * canvasSize.Height),
                       new PointSingle(ProjectiveNewYPointX * canvasSize.Width, ProjectiveNewYPointY * canvasSize.Height),
                       ProjectiveNewXWeight / canvasSize.Width,
                       ProjectiveNewYWeight / canvasSize.Height,
                       ProjectiveStartPointWeight);

                _figureProjectiveTransform = projectiveTransform;
                RebuildFigureTransform();

                await RedrawAllAsync();
            }
        }

        async Task ApplyProjectiveTransformWithDelayAsync()
        {
            await WaitBeforeExecutionAsync(ApplyProjectiveTransformAsync);
        }

        async Task ResetProjectiveTransformAsync()
        {

            ResetProjectiveTransforms();
            await ApplyProjectiveTransformAsync();
        }
    }
}
