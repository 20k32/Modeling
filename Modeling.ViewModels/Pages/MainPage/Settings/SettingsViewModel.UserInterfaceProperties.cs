using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Modeling.Core.Constants;
using Modeling.Core.Drawing.Providers;
using Modeling.Core.Messages.Parameters.Canvas.Settings;
using Modeling.Core.Messages.ViewModels;
using Modeling.Models.Settings;
using System.Collections.ObjectModel;
using System.Linq;

namespace Modeling.ViewModels.Pages.MainPage.Settings
{
    public sealed partial class SettingsViewModel : BaseViewModel
    {
        [ObservableProperty]
        double _canvasHeight;

        [ObservableProperty]
        double _canvasWidth;

        [ObservableProperty]
        bool _drawFigureBounds;

        [ObservableProperty]
        bool _drawFigureShapeBounds;

        [ObservableProperty]
        bool _drawAxis;

        [ObservableProperty]
        bool _drawAxisMarks;

        [ObservableProperty]
        bool _drawAxisArrows;

        [ObservableProperty]
        bool _drawGrid;

        [ObservableProperty]
        bool _attachGridToFigure;

        [ObservableProperty]
        float _pixelsPerCentimeter;

        [ObservableProperty]
        RefreshRateItem _refreshRate;

        [ObservableProperty]
        ObservableCollection<RefreshRateItem> _refreshRates;

        async partial void OnRefreshRateChanged(RefreshRateItem value)
        {
            if (value is null)
            {
                value = RefreshRates.First(refreshRate => refreshRate.Value == DrawingConstants.DEFAULT_CANVAS_REFRESH_RATE);
            }

            if (value.Value != _drawingSettingsProvider.Settings.CanvasRefreshRate)
            {
                await WaitBeforeExecutionAsync(() =>
                {
                    if (value.Value != _drawingSettingsProvider.Settings.CanvasRefreshRate)
                    {
                        var settingsParameter = new UpdateDrawingsParameter(
                            shouldUpdateCanvasSize: false,
                            shouldRecreateFigure: false,
                            shouldUpdateRefreshRate: true,
                            new((float)CanvasWidth,
                            (float)CanvasHeight), PixelsPerCentimeter,
                            value.Value);

                        WeakReferenceMessenger.Default.Send(new ChangeCanvasSettingsMessage(this, settingsParameter));

                        _drawingSettingsProvider.Settings.CanvasRefreshRate = value.Value;
                    }
                });
            }
        }

        async partial void OnPixelsPerCentimeterChanged(float value)
        {
            if (value > DrawingConstants.MAXIMUM_CANVAS_PIXELS_PER_CENTIMETER)
            {
                value = DrawingConstants.MAXIMUM_CANVAS_PIXELS_PER_CENTIMETER;
                _pixelsPerCentimeter = value;
                OnPropertyChanged(nameof(PixelsPerCentimeter));
            }

            if (value < DrawingConstants.MINIMUM_CANVAS_PIXELS_PER_CENTIMETER)
            {
                value = DrawingConstants.MINIMUM_CANVAS_PIXELS_PER_CENTIMETER;
                _pixelsPerCentimeter = value;
                OnPropertyChanged(nameof(PixelsPerCentimeter));
            }

            if (value != _drawingSettingsProvider.Settings.PixelsPerCentimeter)
            {
                await WaitBeforeExecutionAsync(() =>
                {
                    if (value != _drawingSettingsProvider.Settings.PixelsPerCentimeter)
                    {
                        var settingsParameter = new UpdateDrawingsParameter(
                            shouldUpdateCanvasSize: false,
                            shouldRecreateFigure: true,
                            shouldUpdateRefreshRate: false,
                            new((float)CanvasWidth,
                            (float)CanvasHeight), PixelsPerCentimeter,
                            RefreshRate.Value);

                        WeakReferenceMessenger.Default.Send(new ChangeCanvasSettingsMessage(this, settingsParameter));

                        _drawingSettingsProvider.Settings.PixelsPerCentimeter = (float)value;
                    }
                });
            }
        }

        async partial void OnCanvasHeightChanged(double value)
        {
            if (value > DrawingConstants.MAXIMUM_CANVAS_SIZE_PIXELS)
            {
                value = DrawingConstants.MAXIMUM_CANVAS_SIZE_PIXELS;
                _canvasHeight = value;
                OnPropertyChanged(nameof(CanvasHeight));
            }

            if (value < DrawingConstants.MINIMUM_CANVAS_SIZE_PIXELS)
            {
                value = DrawingConstants.MINIMUM_CANVAS_SIZE_PIXELS;
                _canvasHeight = value;
                OnPropertyChanged(nameof(CanvasHeight));
            }

            var newSize = new Windows.Graphics.SizeInt32(
                    _drawingSettingsProvider.Settings.CanvasSize.Width,
                    (int)value);

            if (newSize != _drawingSettingsProvider.Settings.CanvasSize)
            {
                await WaitBeforeExecutionAsync(() =>
                {
                    if (newSize != _drawingSettingsProvider.Settings.CanvasSize)
                    {
                        var settingsParameter = new UpdateDrawingsParameter(
                            shouldUpdateCanvasSize: true,
                            shouldRecreateFigure: true,
                            shouldUpdateRefreshRate: false,
                            new((float)CanvasWidth, (float)CanvasHeight),
                            PixelsPerCentimeter,
                            RefreshRate.Value);

                        WeakReferenceMessenger.Default.Send(new ChangeCanvasSettingsMessage(this, settingsParameter));

                        _drawingSettingsProvider.Settings.CanvasSize = newSize;
                    }
                });
            }
        }

        async partial void OnCanvasWidthChanged(double value)
        {
            if (value > DrawingConstants.MAXIMUM_CANVAS_SIZE_PIXELS)
            {
                value = DrawingConstants.MAXIMUM_CANVAS_SIZE_PIXELS;
                _canvasWidth = value;
                OnPropertyChanged(nameof(CanvasWidth));
            }

            if (value < DrawingConstants.MINIMUM_CANVAS_SIZE_PIXELS)
            {
                value = DrawingConstants.MINIMUM_CANVAS_SIZE_PIXELS;
                _canvasWidth = value;
                OnPropertyChanged(nameof(CanvasWidth));
            }

            var newSize = new Windows.Graphics.SizeInt32(
                    (int)value,
                    _drawingSettingsProvider.Settings.CanvasSize.Height);

            if (newSize != _drawingSettingsProvider.Settings.CanvasSize)
            {
                await WaitBeforeExecutionAsync(() =>
                {
                    if (newSize != _drawingSettingsProvider.Settings.CanvasSize)
                    {
                        var updateDrawingsParameter = new UpdateDrawingsParameter(
                            shouldUpdateCanvasSize: true,
                            shouldRecreateFigure: true,
                            shouldUpdateRefreshRate: false,
                            new((float)CanvasWidth, (float)CanvasHeight),
                            PixelsPerCentimeter,
                            RefreshRate.Value);

                        WeakReferenceMessenger.Default.Send(new ChangeCanvasSettingsMessage(this, updateDrawingsParameter));

                        _drawingSettingsProvider.Settings.CanvasSize = newSize;
                    }
                });
            }
        }

        partial void OnAttachGridToFigureChanged(bool value)
        {
            _drawingSettingsProvider.Settings.AttachGridToFigure = value;
        }

        partial void OnDrawGridChanged(bool value)
        {
            _drawingSettingsProvider.Settings.DrawGrid = value;
        }

        partial void OnDrawAxisChanged(bool value)
        {
            _drawingSettingsProvider.Settings.DrawAxis = value;
        }

        partial void OnDrawAxisMarksChanged(bool value)
        {
            _drawingSettingsProvider.Settings.DrawAxisMarks = value;
        }

        partial void OnDrawAxisArrowsChanged(bool value)
        {
            _drawingSettingsProvider.Settings.DrawAxisArrows = value;
        }

        partial void OnDrawFigureShapeBoundsChanged(bool value)
        {
            _drawingSettingsProvider.Settings.DrawFigureShapeBounds = value;
        }

        partial void OnDrawFigureBoundsChanged(bool value)
        {
            _drawingSettingsProvider.Settings.DrawFigureBounds = value;
        }
    }
}
