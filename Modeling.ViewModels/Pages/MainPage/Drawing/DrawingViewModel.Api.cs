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
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Core.Messages.Canvas.Settings;
using Modeling.Core.Messages.Parameters.Canvas.Drawing;
using Modeling.Core.Messages.Parameters.Canvas.Settings;
using Modeling.Core.Messages.Settings;
using Modeling.Core.Messages.ViewModels;
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
using Modeling.Models.UserInterface;
using Modeling.ViewModels.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics;

namespace Modeling.ViewModels.Pages.MainPage.Drawing
{
    public sealed partial class DrawingViewModel : ObservableObject
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

        PointSingle startDrawingPoint;

        UserPointDrawingAction _drawingAction;
        PointSingle _previousMovedPoint;

        Matrix3x3Single _gridTransform;
        Matrix3x3Single _userPointTransform;
        Matrix3x3Single _figureTransform;

        IPointGeometry _pointerMoveNearestSegment;

        PointListTransformMessageParameter _lastDrawingMessage;

        bool _shouldChangeFigurePosition;
        bool _shouldApplyGeneralTransformForUserPoint;
        bool _canChangePositionForUserPoint;
        bool _canRedrawUserPoint;

        float _lastRotationAngle;

        CancellationTokenSource _rotateAnimationCancellationSource;

        MouseCursor _cursorState;

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
            _figureTransform = DrawingConstants.NON_TRANSFORM_MATRIX;

            _drawingSettingsProvider = Ioc.Default.GetRequiredService<IDrawingSettingsProvider>();
            _drawingSettingsProvider.SettingsChanged += OnDrawingSettingsProviderSettingsChanged;

            WeakReferenceMessenger.Default.Register<ChangeCanvasSettingsMessage>(this, OnChangeCanvasSettingsMessage);

            _navigationProvider = Ioc.Default.GetRequiredService<INavigationProvider>();
            _navigationProvider.Navigated += OnNavigationProviderNavigated;

            _pickButtonsVisible = true;

            _userPoint = Ioc.Default.GetService<IPointGeometryCreationFactory>()
                .CreateCirclePointGeometry(DrawingConstants.DEFAULT_POINT, SegmentDimensionParameter.None);
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
            CircleEditingPanelVisible = false;
            LineEditingPanelVisible = false;

            PositionEditingPanelVisible = false;
            SizeEditingPanelVisible = false;

            PickButtonsVisible = true;
            ChangeFigurePosition = false;
            PickShapeForResizing = false;
            CancelButtonVisible = false;
            RotationPointVisible = false;

            _canRedrawUserPoint = false;
            _shouldApplyGeneralTransformForUserPoint = true;
            _drawingAction = UserPointDrawingAction.None;
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
                RedrawAll();
            }
        }

        void EndSegmentSelection()
        {
            NearestSegment = _pointerMoveNearestSegment;
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

            if (shouldHandlePoint && nearestSegment != _pointerMoveNearestSegment)
            {
                _pointerMoveNearestSegment = nearestSegment;

                var segmentRedrawingMessageParameter = _lastDrawingMessage.With(
                    points: nearestSegment.Bounds.GetPointsFromBounds(),
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

        void RedrawUserPointCore(PointSingle point)
        {
            if (!_canChangePositionForUserPoint)
            {
                ChangeMouseCursorForUserPoint(_userPoint, point);
                return;
            }

            if (point != _previousMovedPoint)
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

                RedrawAll();
            }
        }

        void RedrawAll()
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
                    points: Ioc.Default.GetRequiredService<IPointListCollection>(),
                    color: drawingColor,
                    transformMatrix: _gridTransform,
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

            if (_canRedrawUserPoint)
            {
                var circleColor = _drawingSettingsProvider.Settings.VerticalAxisColor;
                var circleDrawingColor = new DrawingColor(circleColor);

                currentDrawingMessage = currentDrawingMessage.With(
                     points: _userPoint.Points,
                     color: circleDrawingColor,
                     transformMatrix: _shouldApplyGeneralTransformForUserPoint ? _gridTransform : _userPointTransform,
                     shouldFillGeometry: true,
                     fillColor: new DrawingColor(_drawingSettingsProvider.Settings.HorizontalAxisColor),
                     clearBeforeRedraw: false,
                     backgroundColor: backgroundColor,
                     thickness: thickness);

                if (_drawingSettingsProvider.Settings.DrawFigureShapeBounds)
                {
                    currentDrawingMessage = currentDrawingMessage.With(
                        points: _userPoint.Bounds.GetPointsFromBounds(),
                        color: new(_drawingSettingsProvider.Settings.FigureBoundsColor),
                        thickness: _drawingSettingsProvider.Settings.FigureDrawingThickness);
                }
            }

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

            _figure.InitializeSegmentDimensions(pixelsPerCentimeter);

            _figure.AddSegments(
                FigureExtensions.CreateCustomShape(startDrawingPoint,
                _figure.GetActualSegmentsDimensions()),
                pixelsPerCentimeter);

            _figure.CalculatePropertiesFromSegments();
            _figure.CalculateDefaultPropertiesFromSegments();
        }

        private PointListTransformMessageParameter GetDrawingFigureMessage(PointListTransformMessageParameter parentMessage)
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

            _horizontalAxisArrows.Clear();
            _verticalAxisArrows.Clear();

            _horizontalAxis.Clear();
            _verticalAxis.Clear();

            _horizontalAxisMarks.Clear();
            _verticalAxisMarks.Clear();

            _figure.Segments.Clear();

            _userPoint.Points.Clear();
        }

        void InitializeDrawingElements(SizeSingle canvasSize, float pixelsPerCentimeter)
        {
            InitializeGrid(canvasSize, pixelsPerCentimeter);
            InitializeAxis(canvasSize, pixelsPerCentimeter);
            InitializeMarksOnAxis(canvasSize, pixelsPerCentimeter);
            InitializeFigure(canvasSize, pixelsPerCentimeter);

            var centerPoint = new PointSingle(canvasSize.Width / 2f, canvasSize.Height / 2f);
            InitializeUserPoint(centerPoint);
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

        void HandleCircleLengthChanged(float value)
        {
            if (NearestSegment is ICirclePointGeometry circleGeometry)
            {
                var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

                var newRadius = value * pixelsPerCentimeter;

                var newPoints = circleGeometry.CenterPoint.GetCirclePoints(newRadius);

                circleGeometry.ClearPoints();

                circleGeometry.AddPointsRange(newPoints);

                circleGeometry.CalculateBounds();

                circleGeometry.CommitPropertyChanges();
            }
        }

        void HandleLineLengthChanged(float value)
        {
            if (NearestSegment is not ILinePointGeometry lineGeometry || value < 0)
            {
                return;
            }

            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;
            var newLength = value * pixelsPerCentimeter;

            /*var p1 = lineGeometry.DefaultPoints.First();
            var p2 = lineGeometry.DefaultPoints.Last();

            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;
            var distance = MathF.Sqrt(dx * dx + dy * dy);

            var ux = distance > 0 ? dx / distance : 1f;
            var uy = distance > 0 ? dy / distance : 0f;

            var midX = (p1.X + p2.X) / 2f;
            var midY = (p1.Y + p2.Y) / 2f;
            var half = newLength / 2f;

            var newFirstPoint = new PointSingle(
                x: midX - ux * half,
                y: midY - uy * half);

            var newSecondPoint = new PointSingle(
                x: midX + ux * half,
                y: midY + uy * half

            lineGeometry.Points.Clear();
            lineGeometry.Points.AddRange([newFirstPoint, newSecondPoint]);

            lineGeometry.CalculateBounds();

            lineGeometry.CommitPropertyChanges(););*/
        }

        void OnNearestSegmentDimensionChanged()
        {
            RedrawAll();
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

            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            point = _figureTransform.Inverse() * point;

            if (_figure.Bounds.Contains(point, pixelsPerCentimeter))
            {
                _shouldChangeFigurePosition = true;

                ChangeMouseCursor(MouseCursor.Move);
            }
        }

        void SelectEntireFigure(PointSingle point)
        {
            if (!_shouldChangeFigurePosition)
            {
                ChangeMouseCursorForEntireFigurePoint(_figure, point);
                return;
            }

            if (point != _previousMovedPoint)
            {
                var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;

                var centerX = canvasSize.Width / 2f;
                var centerY = canvasSize.Height / 2f;

                var offsetX = point.X - centerX;
                var offsetY = point.Y - centerY;

                _figureTransform = MatrixExtensions.CreateTranslationTransform(
                    offsetX,
                    offsetY);

                _previousMovedPoint = point;

                Logger.Information($"X: {point.X} Y: ; {point.Y}");

                _shouldApplyGeneralTransformForUserPoint = false;

                RedrawAll();
            }
        }

        void EndFigureSelection(PointSingle point)
        {
            _shouldChangeFigurePosition = false;
            ChangeMouseCursor(MouseCursor.Default);
        }

        void RotateFigure(float angleDegrees)
        {

            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;
            var pixelsPerMillimeter = pixelsPerCentimeter / 10;

            var rotationAngle = angleDegrees - _lastRotationAngle;
            _lastRotationAngle = angleDegrees;

            var transformedUserPoint = _userPointTransform * _userPoint.CenterPoint;
            _figureTransform = MatrixExtensions.CreateRotationTransform(transformedUserPoint, -rotationAngle.DegreesToRadian()) * _figureTransform;

            RedrawAll();
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

            var stepFrequency = 1;//userInterfaceConstants.SlidersStepFrequency;
            var animationTimeoutMilliseconds = userInterfaceConstants.AnimationTimeoutMilliseconds;

            var rotationAngle = _lastRotationAngle;

            var needToIncreaseAngle = rotationAngle + 1 < DrawingConstants.CIRCLE_END_ANGLE_DEGREES;
            var needToDecreaseAngle = !needToIncreaseAngle;

            while (!token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();

                SetRotationAngleSilent(rotationAngle);

                RotateFigure(rotationAngle);

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
    }
}
