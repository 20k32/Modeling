using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Modeling.Core.Drawing;
using Modeling.Core.Logging;
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
            NearestSegment = _pointerMoveNearestSegment;

            switch (_drawingAction)
            {
                case UserPointDrawingAction.AxisPointSelection: RedrawUserPointCore(point); break;
                case UserPointDrawingAction.FigurePointSelection: EndSegmentSelection(); break;
                default: break;
            }
        }

        [RelayCommand]
        async Task InitializeCanvasAsync()
        {
            await LoadSettingsAsync();

            LoadCanvasState();

            RedrawFigure();
        }

        [RelayCommand]
        void CancelPickingShapeCommand()
        {

        }
    }
}
