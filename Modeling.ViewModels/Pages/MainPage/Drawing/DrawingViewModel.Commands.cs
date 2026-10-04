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
        void DrawLines()
        {
            _drawingAction = UserPointDrawingAction.AxisPointSelection;
            RedrawAll();
        }

        [RelayCommand]
        void CanvasPointerMoved(PointSingle point)
        {
            switch (_drawingAction)
            {
                case UserPointDrawingAction.AxisPointSelection: RedrawUserPointCore(point); break;
                case UserPointDrawingAction.FigurePointSelection: SelectSegmentOnFigure(point); break;
                case UserPointDrawingAction.EntireFigureSelection: SelectEntireFigure(point); break;
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

            RedrawAll();
        }

        [RelayCommand]
        void CancelPickingShape()
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

            if (NearestSegment is not null)
            {
                NearestSegment = default;
                RedrawAll();
            }
        }

        [RelayCommand]
        void ResetFigure()
        {
            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;
            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            ClearDrawingElements();
            InitializeDrawingElements(new SizeSingle(canvasSize.Width, canvasSize.Height),
                pixelsPerCentimeter);

            _shouldApplyGeneralTransformForUserPoint = true;

            _figureTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _userPointTransform = DrawingConstants.NON_TRANSFORM_MATRIX;
            _gridTransform = DrawingConstants.NON_TRANSFORM_MATRIX;

            RedrawAll();
        }
    }
}
