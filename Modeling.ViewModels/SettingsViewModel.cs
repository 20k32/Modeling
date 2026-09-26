using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Modeling.Core.Messages.Settings;
using Modeling.Core.Extensions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Drawing.Providers;
using Modeling.Core.Abstractions.Providers;
using Modeling.Core.Constants;
using Modeling.Core.Miscellaneous;
using System.Reflection.Metadata;

namespace Modeling.ViewModels
{
    public sealed partial class SettingsViewModel : ObservableObject
    {
        readonly IApplicationSettingsProvider _applicationSettingsProvider;
        readonly IDrawingSettingsProvider _drawingSettingsProvider;

        [ObservableProperty]
        double _canvasHeight;

        [ObservableProperty]
        double _canvasWidth;


        bool _initialized;

        public SettingsViewModel()
        {
            _canvasHeight = DrawingConstants.CANVAS_SIZE.Height;
            _canvasWidth = DrawingConstants.CANVAS_SIZE.Width;

            _drawingSettingsProvider = Ioc.Default.GetService<IDrawingSettingsProvider>();
            _applicationSettingsProvider = Ioc.Default.GetService<IApplicationSettingsProvider>();
        }

        [RelayCommand]
        void Initialize()
        {
            WeakReferenceMessenger.Default.Register<InitializeSettingsMessage>(this, OnSettingsViewModelInitializeSettingsMessage);
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

            var firstApplicationLaunch = _applicationSettingsProvider
                .GetSettingsValue<bool?>(CoreConstants.FIRST_LAUNCH_APPLICAITON_SETTING_KEY);

            var shouldInitializeSettingsWithDefaults = !(firstApplicationLaunch ?? false)
                || !(_drawingSettingsProvider.Settings.Initialized ?? false);

            if (shouldInitializeSettingsWithDefaults)
            {
                if (firstApplicationLaunch ?? true)
                {
                    _applicationSettingsProvider.SetSettingsValue(CoreConstants.FIRST_LAUNCH_APPLICAITON_SETTING_KEY, false);
                }

                SetDefaultSettings();

                await SaveSettingsAsync();
            }

            ApplySettings();

            return Unit.Default;
        }

        async Task<Unit> SaveSettingsAsync()
        {
            await _drawingSettingsProvider.SaveSettingsAsync();

            return Unit.Default;
        }

        void ApplySettings()
        {
            CanvasHeight = _drawingSettingsProvider.Settings.CanvasSize.Height;
            CanvasWidth = _drawingSettingsProvider.Settings.CanvasSize.Width;
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

            _drawingSettingsProvider.Settings.RefreshRate = DrawingConstants.DEFAULT_REFRESH_RATE;

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
        }
    }
}
