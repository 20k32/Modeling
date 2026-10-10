using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Base.SynchronousMessages;
using System;
using System.Threading.Tasks;

namespace Modeling.ViewModels.Pages.MainPage.Drawing
{
    public sealed partial class DrawingViewModel : ObservableObject
    {
        [RelayCommand]
        void Initialize()
        {
            Logger.LoadedInformation("Main page");

            InitializeCanvas();

            InitializeDrawingSession();

            InitializeSettings();
        }

        [RelayCommand]
        async Task DrawLinesAsync()
        {
            _drawingAction = UserPointDrawingAction.AxisPointSelection;
            await RedrawAllAsync();
        }

        [RelayCommand]
        async Task CanvasPointerMovedAsync(PointSingle point)
        {
            switch (_drawingAction)
            {
                case UserPointDrawingAction.AxisPointSelection: await RedrawUserPointCoreAsync(point); break;
                case UserPointDrawingAction.FigurePointSelection: SelectSegmentOnFigure(point); break;
                case UserPointDrawingAction.EntireFigureSelection: await SelectEntireFigureAsync(point); break;
                default: break;
            }
        }

        [RelayCommand]
        void CanvasPointerPressed(PointSingle point)
        {
            switch (_drawingAction)
            {
                case UserPointDrawingAction.AxisPointSelection: StartUserPointSelection(point); break;
                case UserPointDrawingAction.FigurePointSelection: EndSegmentSelection(); break;
                case UserPointDrawingAction.EntireFigureSelection: StartFigureSelection(point); break;
                default: break;
            }
        }

        [RelayCommand]
        void CanvasPointerReleased(PointSingle point)
        {
            switch (_drawingAction)
            {
                case UserPointDrawingAction.AxisPointSelection: EndUserPointRedrawing(point); break;
                case UserPointDrawingAction.EntireFigureSelection: EndFigureSelection(point); break;
                default: break;
            }
        }

        [RelayCommand]
        async Task InitializeCanvasAsync()
        {
            await LoadSettingsAsync();

            LoadCanvasState();

            await RedrawAllAsync();
        }

        [RelayCommand]
        async Task CancelPickingShape()
        {
            _previousMovedPoint = DrawingConstants.BREAK_POINT;

            _shouldChangeFigurePosition = false;
            _canRedrawUserPoint = false;

            _wasPositionEditingControlVisible = false;
            _wasRotationPointVisible = false;

            _drawingAction = UserPointDrawingAction.None;

            PositionEditingPanelVisible = false;
            SizeEditingPanelVisible = false;

            PickButtonsVisible = true;

            CancelButtonVisible = false;

            ChangeFigurePosition = false;

            PositionEditingControlVisible = false;

            AnimateRotation = false;

            if (NearestSegment is not null)
            {
                NearestSegment = default;
                await RedrawAllAsync();
            }
        }

        [RelayCommand]
        async Task ResetFigureAsync()
        {
            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;
            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            ClearDrawingElements();
            InitializeDrawingElements(new SizeSingle(canvasSize.Width, canvasSize.Height),
                pixelsPerCentimeter);

            _shouldApplyGeneralTransformForUserPoint = true;

            _canRedrawUserPoint = false;

            SetRotationAngleSilent(0);

            _figureRotationTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _figureRotationTransform = DrawingConstants.NON_TRANSFORM_MATRIX;

            _figureTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _figureTransformCopy = DrawingConstants.NON_TRANSFORM_MATRIX;

            _userPointTransform = DrawingConstants.NON_TRANSFORM_MATRIX;

            _gridTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _gridTransformCopy = DrawingConstants.NON_TRANSFORM_MATRIX;

            SetRotationPointSilent(false);

            await RedrawAllAsync();
        }

        [RelayCommand]
        async Task ApplyAffineTransform()
        {
            var origin = new PointSingle(AffineStartPointX, AffineStartPointY);

            var xAxis = new PointSingle(1 + AffineNewXPointX, AffineNewXPointY);

            var yAxis = new PointSingle(AffineNewYPointX, 1 + AffineNewYPointY);

            var affineTransformMatrix = MatrixExtensions.CreateAffineTransform(origin, xAxis, yAxis);

            _figureTransform = affineTransformMatrix * _figureTransformCopy;
            _gridTransform = affineTransformMatrix * _gridTransformCopy;

            await RedrawAllAsync();
        }
    }
}
