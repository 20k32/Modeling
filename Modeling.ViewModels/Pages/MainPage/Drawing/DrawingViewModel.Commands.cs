using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        [RelayCommand]
        void CanvasPointerReleased(PointSingle point)
        {
            switch (_drawingAction)
            {
                case UserPointDrawingAction.AxisPointSelection: EndUserPointRedrawing(point); break;
                default: break;
            }
        }

        private void EndUserPointRedrawing(PointSingle point)
        {
            _drawingAction = UserPointDrawingAction.None;
        }

        [RelayCommand]
        async Task InitializeCanvasAsync()
        {
            await LoadSettingsAsync();

            LoadCanvasState();

            RedrawFigure();
        }

        [RelayCommand]
        void CancelPickingShape()
        {
            PositionEditingPanelVisible = false;
            SizeEditingPanelVisible = false;

            PickButtonsVisible = true;

            CancelButtonVisible = false;

            ChangeFigurePosition = false;

            PositionEditingControlVisible = false;

            if (NearestSegment is not null)
            {
                NearestSegment = default;
                RedrawFigure();
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

            RedrawFigure();
        }
    }
}
