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
    public sealed partial class DrawingViewModel : BaseViewModel
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
            if (_isCurveInitialized)
            {
                await HandleCurvePointerMovedAsync(point);
                return;
            }

            switch (_drawingAction)
            {
                case UserPointDrawingAction.AxisPointSelection: await RedrawUserPointCoreAsync(point); break;
                case UserPointDrawingAction.FigurePointSelection: SelectSegmentOnFigure(point); break;
                case UserPointDrawingAction.EntireFigureSelection: await SelectEntireFigureAsync(point); break;
                default: break;
            }
        }

        [RelayCommand]
        async Task CanvasPointerPressedAsync(PointSingle point)
        {
            if (_isCurveInitialized)
            {
                await HandleCurvePointerPressedAsync(point);
                return;
            }

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
            if (_isCurveInitialized)
            {
                HandleCurvePointerReleased();
                return;
            }

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
        }

        [RelayCommand]
        async Task CancelPickingShape()
        {
            PositionEditingPanelVisible = false;
            SizeEditingPanelVisible = false;

            PickButtonsVisible = true;

            CancelButtonVisible = false;

            ChangeFigurePosition = false;

            PositionEditingControlVisible = false;

            AnimateRotation = false;

            NearestSegment = default;

            if (NearestSegment is not null)
            {
                await RedrawAllAsync();
            }

            await CancelApiLayerAsync();
        }

        [RelayCommand]
        async Task ResetFigureAsync()
        {
            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;
            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            ClearDrawingElements();

            InitializeDrawingElements(new SizeSingle(canvasSize.Width, canvasSize.Height),
                pixelsPerCentimeter);

            SetRotationAngleSilent(0);

            SetRotationPointControlVisibleSilent(false);

            await ResetApiLayerAsync();
        }

        [RelayCommand]
        async Task ApplySymmetryTransformAsync()
        {
            await ApplySymmetryTransformRelativeRotationPointAsync();
        }

        [RelayCommand]
        async Task ApplyDefaultProjectiveTransformAsync()
        {
            await ResetProjectiveTransformAsync();
        }

        [RelayCommand]
        async Task StartCurveAnimationAsync()
        {
            await StartCurveAnimationCoreAsync();
        }

        [RelayCommand]
        async Task PauseCurveAnimationAsync()
        {
            await StopCurveAnimationAsync();
        }

        [RelayCommand]
        async Task ResetCurveAnimationAsync()
        {
            await ResetCurveAnimationCoreAsync();
        }

        [RelayCommand]
        async Task ResetCurveTransformsAsync()
        {
            await ResetCurveTransformsCoreAsync();
        }
    }
}
