using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Drawing.Providers;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Core.Messages.Canvas.Settings;
using Modeling.Core.Messages.Parameters.Canvas;
using Modeling.Core.Messages.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Modeling.ViewModels
{
    public sealed partial class DrawingViewModel : ObservableObject
    {
        readonly IDrawingSettingsProvider _drawingSettingsProvider;

        readonly List<PointSingle> _grid;
        readonly List<PointSingle> _axis;
        readonly List<PointSingle> _figure;

        public DrawingViewModel()
        {
            _figure = [];
            _grid = [];
            _drawingSettingsProvider = Ioc.Default.GetRequiredService<IDrawingSettingsProvider>();
        }

        [RelayCommand]
        void Initialize()
        {
            Logger.LoadedInformation("Main page");

            InitializeCanvas();

            InitializeDrawingSession();

            InitializeSettings();

            _figure.Add(new PointSingle(0, 0));
            _figure.Add(new PointSingle(10, 10));
            _figure.Add(new PointSingle(100, 100));
        }

        [RelayCommand]
        void DrawLines()
        {
            DrawGrid();

            /*var rawBackgroundColor = _drawingSettingsProvider.Settings.BackgroundColor;
            var backgroundColor = new DrawingColor(rawBackgroundColor);

            var rawDrawingColor = _drawingSettingsProvider.Settings.DrawingColor;
            var drawingColor = new DrawingColor(rawDrawingColor);

            var thickness = _drawingSettingsProvider.Settings.DrawingThickness;

            var transformMessageParameter = new PointListTransformMessageParameter(
                points: _figure,
                color: drawingColor,
                transformMatrix: transformMatrix * MatrixExtensions.CreateRotationTransform(rotationAngle.DegreesToRadian()),
                thickness: thickness,
                clearBeforeRedraw: true,
                backgroundColor: backgroundColor);

            WeakReferenceMessenger.Default.Send(new TransformPointsMessage(this, transformMessageParameter));*/
        }

        void DrawGrid()
        {
            _grid.Clear();

            InitializeGrid();

            var rawBackgroundColor = _drawingSettingsProvider.Settings.BackgroundColor;
            var backgroundColor = new DrawingColor(rawBackgroundColor);

            var rawDrawingColor = _drawingSettingsProvider.Settings.DrawingColor;
            var drawingColor = new DrawingColor(rawDrawingColor);

            var thickness = _drawingSettingsProvider.Settings.DrawingThickness;

            var connectPointsMessageParameter = new PointListMessageParameter(
                _grid,
                drawingColor,
                clearBeforeRedraw: true,
                backgroundColor: backgroundColor,
                thickness: thickness);

            WeakReferenceMessenger.Default.Send(new ConnectPointsMessage(this, connectPointsMessageParameter));
        }

        [RelayCommand]
        async Task InitializeCanvasAsync()
        {
            await LoadSettingsAsync();

            LoadCanvasState();

            InitializeGrid();
        }

        void InitializeGrid()
        {
            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;
            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            for (var y = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                y <= canvasSize.Height; y += (int)pixelsPerCentimeter)
            {
                for (var x = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                    x <= canvasSize.Width; x += (int)pixelsPerCentimeter)
                {
                    _grid.Add(new PointSingle(x, y));
                }

                if (y < canvasSize.Height)
                {
                    _grid.Add(DrawingConstants.INVALID_POINT);
                }
            }

            for (var x = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                x <= canvasSize.Width; x += (int)pixelsPerCentimeter)
            {
                for (var y = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                    y <= canvasSize.Height + pixelsPerCentimeter; y += (int)pixelsPerCentimeter)
                {
                    _grid.Add(new PointSingle(x, y));
                }

                if (x < canvasSize.Width)
                {
                    _grid.Add(DrawingConstants.INVALID_POINT);
                }
            }
        }

        void InitializeAxis()
        {
            var canvasSize = _drawingSettingsProvider.Settings.CanvasSize;
            var pixelsPerCentimeter = _drawingSettingsProvider.Settings.PixelsPerCentimeter;

            for (var y = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                y <= canvasSize.Height; y += (int)pixelsPerCentimeter)
            {
                for (var x = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                    x <= canvasSize.Width; x += (int)pixelsPerCentimeter)
                {
                    _grid.Add(new PointSingle(x, y));
                }

                if (y < canvasSize.Height)
                {
                    _grid.Add(DrawingConstants.INVALID_POINT);
                }
            }

            for (var x = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                x <= canvasSize.Width; x += (int)pixelsPerCentimeter)
            {
                for (var y = DrawingConstants.START_POINT_DRAWING_COORDINATE_X_Y;
                    y <= canvasSize.Height + pixelsPerCentimeter; y += (int)pixelsPerCentimeter)
                {
                    _grid.Add(new PointSingle(x, y));
                }

                if (x < canvasSize.Width)
                {
                    _grid.Add(DrawingConstants.INVALID_POINT);
                }
            }
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

        async Task LoadSettingsAsync()
        {
            await WeakReferenceMessenger.Default.Send(new LoadSettingsAsyncMessage(this));
        }
    }
}
