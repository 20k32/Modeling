using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Drawing.Providers;
using Modeling.Core.Logging;
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
        readonly List<PointSingle> _figure;

        public DrawingViewModel()
        {
            _figure = [];
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
            var color = new DrawingColor(Colors.Yellow);
            var thickness = 1f;

            var connectPointsMessageParameter = new PointListMessageParameter(
                points: _figure,
                color: color,
                thickness: thickness);

            WeakReferenceMessenger.Default.Send(new ConnectPointsMessage(this, connectPointsMessageParameter));
        }

        [RelayCommand]
        async Task InitializeCanvasAsync()
        {
            await LoadSettingsAsync();

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
            WeakReferenceMessenger.Default.Send(new ClearCanvasMessage(this, color));
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
