using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Modeling.Core.Abstractions.Providers;
using Modeling.Core.Constants;
using Modeling.Core.CoreDelegates;
using Modeling.Core.Drawing.Providers;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Parameters.Canvas.Settings;
using Modeling.Core.Messages.Settings;
using Modeling.Core.Messages.ViewModels;
using Modeling.Core.Miscellaneous;
using Modeling.Models.Settings;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.ViewModels.Pages.MainPage.Settings
{
    public sealed partial class SettingsViewModel : ObservableObject
    {
        readonly SemaphoreSlim _applyingChangesLock;
        readonly IApplicationSettingsProvider _applicationSettingsProvider;
        readonly IDrawingSettingsProvider _drawingSettingsProvider;

        CancellationTokenSource _applyingChangesCancellation;

        bool _initialized;

        public SettingsViewModel()
        {
            _applyingChangesLock = new SemaphoreSlim(1, 1);
            _drawingSettingsProvider = Ioc.Default.GetService<IDrawingSettingsProvider>();
            _applicationSettingsProvider = Ioc.Default.GetService<IApplicationSettingsProvider>();
            _refreshRates =
                [
                    new RefreshRateItem(1),
                    new RefreshRateItem(24),
                    new RefreshRateItem(30),
                    new RefreshRateItem(60),
                    new RefreshRateItem(120),
                ];
        }

        async Task WaitBeforeExecutionAsync(ActionEventHandler action)
        {
            try
            {
                await _applyingChangesCancellation.TryCancelAsync(shouldDispose: false);

                using (var cancellationSource = new CancellationTokenSource())
                {
                    _applyingChangesCancellation = cancellationSource;

                    await Task.Delay(CoreConstants.MAXIMUM_DELAY_BEFORE_CHANGES_APPLIED_MILLISECONDS, cancellationSource.Token);

                    await _applyingChangesLock.WaitAsync();

                    action();
                }
            }
            catch (Exception ex)
            {
                if (ex is not OperationCanceledException)
                {
                    Logger.Exception(ex);
                }
            }
            finally
            {
                _applyingChangesLock.ReleaseSafe();
            }
        }

        void OnSettingsViewModelInitializeSettingsMessage(object recipient, InitializeSettingsMessage message)
        {
            if (!_initialized && message.ApplyBasicMessageValidation(recipient))
            {
                _initialized = true;

                RegisterHandlers();
            }
        }

        void RegisterHandlers()
        {
            WeakReferenceMessenger.Default.Register<SaveSettingsAsyncMessage>(this, OnSettingsViewModelSaveSettingsAsync);
            WeakReferenceMessenger.Default.Register<LoadSettingsAsyncMessage>(this, OnSettingsViewModelLoadSettingsAsync);
        }

        void OnSettingsViewModelLoadSettingsAsync(object recipient, LoadSettingsAsyncMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                message.Reply(LoadSettingsAsync());
            }
        }

        void OnSettingsViewModelSaveSettingsAsync(object recipient, SaveSettingsAsyncMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                message.Reply(SaveSettingsAsync());
            }
        }

        async Task InitializeDrawingSettingsAsync()
        {
            await _drawingSettingsProvider.InitializeAsync();

            await _drawingSettingsProvider.LoadSettingsAsync();
        }

        async Task<Unit> LoadSettingsAsync()
        {
            await InitializeDrawingSettingsAsync();

            try
            {
                _drawingSettingsProvider.ShouldInvokeSettingsChanged = false;

                await LoadSettingsCoreAsync();
            }
            finally
            {
                _drawingSettingsProvider.ShouldInvokeSettingsChanged = true;
            }

            return Unit.Default;
        }

        async Task LoadSettingsCoreAsync()
        {
            var firstApplicationLaunch = _applicationSettingsProvider
                .GetSettingsValue<bool?>(CoreConstants.FIRST_LAUNCH_APPLICAITON_SETTING_KEY);

            var shouldInitializeSettingsWithDefaults = (firstApplicationLaunch ?? false)
                || !(_drawingSettingsProvider.Settings.Initialized ?? false);

            if (shouldInitializeSettingsWithDefaults)
            {
                if (!(firstApplicationLaunch ?? false))
                {
                    _applicationSettingsProvider.SetSettingsValue(CoreConstants.FIRST_LAUNCH_APPLICAITON_SETTING_KEY, false);
                }

                SetDefaultSettings();

                await SaveSettingsAsync();
            }

            ApplySettingsForUserInterface();
        }

        async Task<Unit> SaveSettingsAsync()
        {
            await _drawingSettingsProvider.SaveSettingsAsync();

            return Unit.Default;
        }

        void SetDefaultSettings()
        {
            _drawingSettingsProvider.Settings.Initialized = true;

            _drawingSettingsProvider.Settings.DpiX = DrawingConstants.STANDART_DPI;
            _drawingSettingsProvider.Settings.DpiY = DrawingConstants.STANDART_DPI;

            _drawingSettingsProvider.Settings.BackgroundColor = DrawingConstants.DEFAULT_BACKGROUND_COLOR;

            _drawingSettingsProvider.Settings.DrawingColor = DrawingConstants.DEFAULT_COLOR;
            _drawingSettingsProvider.Settings.GridDrawingThickness = DrawingConstants.GRID_DRAWING_THICKNESS;
            _drawingSettingsProvider.Settings.FigureDrawingThickness = DrawingConstants.FIGURE_DRAWING_THICKNESS;

            _drawingSettingsProvider.Settings.Scale = DrawingConstants.DEFAULT_SCALE;

            _drawingSettingsProvider.Settings.CenterCanvasPosition = DrawingConstants.DEFAULT_CENTER_CANVAS_POSITION;
            _drawingSettingsProvider.Settings.RotatePointPosition = DrawingConstants.DEFAULT_ROTATE_POINT_POSITION;

            _drawingSettingsProvider.Settings.Figure = [];
            _drawingSettingsProvider.Settings.HorizontalAxis = [];
            _drawingSettingsProvider.Settings.VerticalAxis = [];

            _drawingSettingsProvider.Settings.DisplayMarkInCanvasCenter = DrawingConstants.DISPLAY_MARK_IN_CANVAS_CENTER_BY_DEFAULT;
            _drawingSettingsProvider.Settings.DisplayAxis = DrawingConstants.DISPLAY_AXIS_BY_DEFAULT;
            _drawingSettingsProvider.Settings.DisplayGrid = DrawingConstants.DISPLAY_GRID_BY_DEFAULT;

            _drawingSettingsProvider.Settings.PixelsPerCentimeter = DrawingConstants.PIXELS_PER_CENTIMETER;
            _drawingSettingsProvider.Settings.CanvasSize = DrawingConstants.CANVAS_SIZE;

            _drawingSettingsProvider.Settings.HorizontalAxisColor = DrawingConstants.X_AXIS_COLOR;
            _drawingSettingsProvider.Settings.VerticalAxisColor = DrawingConstants.Y_AXIS_COLOR;

            _drawingSettingsProvider.Settings.HorizontalAxisTicksColor = DrawingConstants.X_AXIS_TICKS_COLOR;
            _drawingSettingsProvider.Settings.VerticalAxisTicksColor = DrawingConstants.Y_AXIS_TICKS_COLOR;

            _drawingSettingsProvider.Settings.AxisTickLength = DrawingConstants.X_Y_AXIS_TICKS_LENGTH_PIXELS;

            _drawingSettingsProvider.Settings.AxisThickness = DrawingConstants.X_Y_AXIS_THICKNESS;
            _drawingSettingsProvider.Settings.AxisTickThickness = DrawingConstants.X_Y_AXIST_TICKS_THICKNESS;

            _drawingSettingsProvider.Settings.FigureBoundsColor = DrawingConstants.FIGURE_BOUNDS_COLOR;
            _drawingSettingsProvider.Settings.FigureCenterPointColor = DrawingConstants.FIGURE_BOUNDS_COLOR;

            _drawingSettingsProvider.Settings.AttachGridToFigure = true;
            _drawingSettingsProvider.Settings.DrawGrid = true;
            _drawingSettingsProvider.Settings.DrawAxis = true;
            _drawingSettingsProvider.Settings.DrawAxisMarks = true;
            _drawingSettingsProvider.Settings.DrawAxisArrows = true;
            _drawingSettingsProvider.Settings.DrawFigureShapeBounds = true;
            _drawingSettingsProvider.Settings.DrawFigureBounds = true;

            _drawingSettingsProvider.Settings.CanvasRefreshRate = DrawingConstants.DEFAULT_CANVAS_REFRESH_RATE;
        }

        void ApplySettingsForUserInterface()
        {
            _canvasHeight = _drawingSettingsProvider.Settings.CanvasSize.Height;
            _canvasWidth = _drawingSettingsProvider.Settings.CanvasSize.Width;
            _drawFigureBounds = _drawingSettingsProvider.Settings.DrawFigureBounds;
            _drawFigureShapeBounds = _drawingSettingsProvider.Settings.DrawFigureShapeBounds;
            _drawAxis = _drawingSettingsProvider.Settings.DrawAxis;
            _drawAxisMarks = _drawingSettingsProvider.Settings.DrawAxisMarks;
            _drawAxisArrows = _drawingSettingsProvider.Settings.DrawAxisArrows;
            _drawGrid = _drawingSettingsProvider.Settings.DrawGrid;
            _attachGridToFigure = _drawingSettingsProvider.Settings.AttachGridToFigure;
            _pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;
            _refreshRate = RefreshRates.First(refreshRate => refreshRate.Value == _drawingSettingsProvider.Settings.CanvasRefreshRate);

            OnPropertyChanged(nameof(DrawFigureBounds));
            OnPropertyChanged(nameof(DrawFigureShapeBounds));
            OnPropertyChanged(nameof(DrawAxis));
            OnPropertyChanged(nameof(DrawAxisArrows));
            OnPropertyChanged(nameof(DrawAxisMarks));
            OnPropertyChanged(nameof(DrawGrid));
            OnPropertyChanged(nameof(AttachGridToFigure));
            OnPropertyChanged(nameof(CanvasWidth));
            OnPropertyChanged(nameof(CanvasHeight));
            OnPropertyChanged(nameof(PixelsPerCentimeter));
            OnPropertyChanged(nameof(RefreshRate));

            var updateDrawingsParameter = new UpdateDrawingsParameter(
                shouldUpdateCanvasSize: true,
                shouldRecreateFigure: true,
                shouldUpdateRefreshRate: true,
                new((float)CanvasWidth, (float)CanvasHeight),
                PixelsPerCentimeter,
                _drawingSettingsProvider.Settings.CanvasRefreshRate);

            WeakReferenceMessenger.Default.Send(new ChangeCanvasSettingsMessage(this, updateDrawingsParameter));
        }
    }
}
