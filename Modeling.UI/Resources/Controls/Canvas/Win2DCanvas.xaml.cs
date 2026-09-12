using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Modeling.Core.Constants;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Core.Messages.Canvas.Settings;
using Modeling.Models.Drawing.DrawingMessageValues;
using Modeling.Models.Drawing.DrawingPipeline;
using Modeling.Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Modeling.UI.Resources.Controls.Canvas
{
    public sealed partial class Win2DCanvas : UserControl
    {
        readonly IDrawingPipeline _drawingPipeline;
        CanvasRenderTarget _canvasRenderTarget;
        bool _isControlInitialized;

        public static readonly DependencyProperty InitializeProperty =
            DependencyProperty.Register(nameof(Initialize),
            typeof(IRelayCommand),
            typeof(Win2DCanvas),
            new PropertyMetadata(default));

        public IRelayCommand Initialize
        {
            get { return (IRelayCommand)GetValue(InitializeProperty); }
            set { SetValue(InitializeProperty, value); }
        }

        public Win2DCanvas()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<InitializeCanvasControlMessage>(this, OnWin2DCanvasInitializeCanvasControlMessage);
            WeakReferenceMessenger.Default.Register<DisposeCanvasControlMessage>(this, OnWin2DCanvasDisposeCanvasControlMessage);

            _drawingPipeline = Ioc.Default.GetRequiredService<IDrawingPipeline>();
        }

        async void OnWin2DCanvasDisposeCanvasControlMessage(object recipient, DisposeCanvasControlMessage message)
        {
            if (_isControlInitialized && message.ApplyBaseMessageValidation(recipient))
            {
                _isControlInitialized = false;
                
                await DisposeDrawingPipelineAsync();

                UnregisterMessages();
            }
        }

        async void OnWin2DCanvasInitializeCanvasControlMessage(object recipient, InitializeCanvasControlMessage message)
        {
            if (!_isControlInitialized && message.ApplyBaseMessageValidation(recipient))
            {
                Logger.InitializedInformation(nameof(Win2DCanvas));

                _isControlInitialized = true;

                RegisterMessages();

                _ = InitializeDrawingPipelineAsync();
            }
            else
            {
                Logger.Information("Canvas control already initialized");
            }
        }

        void RegisterMessages()
        {
            WeakReferenceMessenger.Default.Register<InitializeDrawingSessionMessage>(this, OnWin2DCanvasInitializeDrawingSessionMessage);
            WeakReferenceMessenger.Default.Register<EndDrawingSessionMessage>(this, OnWin2DCanvasEndDrawingSessionMessage);
            WeakReferenceMessenger.Default.Register<ConnectTwoPointsMessage>(this, OnWin2DCanvasConnectTwoPointsMessage);
            WeakReferenceMessenger.Default.Register<ClearCanvasMessage>(this, OnWin2DCanvasClearCanvasMessage);
        }

        void UnregisterMessages()
        {
            WeakReferenceMessenger.Default.Unregister<InitializeDrawingSessionMessage>(this);
            WeakReferenceMessenger.Default.Unregister<EndDrawingSessionMessage>(this);
            WeakReferenceMessenger.Default.Unregister<ConnectTwoPointsMessage>(this);
            WeakReferenceMessenger.Default.Unregister<ClearCanvasMessage>(this);
        }

        void OnWin2DCanvasClearCanvasMessage(object recipient, ClearCanvasMessage message)
        {
            if (message.ApplyBaseMessageValidation(recipient))
            {
                EnqueueMessageToPipeline(message);
            }
        }

        void OnWin2DCanvasConnectTwoPointsMessage(object recipient, ConnectTwoPointsMessage message)
        {
            if (message.ApplyBaseMessageValidation(recipient))
            {
                EnqueueMessageToPipeline(message);
            }
        }

        void OnWin2DCanvasEndDrawingSessionMessage(object recipient, EndDrawingSessionMessage message)
        {
            if (message.ApplyBaseMessageValidation(recipient))
            {
                AnimatedCanvas.Draw -= OnCanvasAnimatedControlDraw;

                DisposeRenderTarget();
            }
        }

        void OnWin2DCanvasInitializeDrawingSessionMessage(object recipient, InitializeDrawingSessionMessage message)
        {
            if (message.ApplyBaseMessageValidation(recipient))
            {
                AnimatedCanvas.CreateResources -= OnCanvasAnimatedControlCreateResources;
                AnimatedCanvas.CreateResources += OnCanvasAnimatedControlCreateResources;

                AnimatedCanvas.Draw -= OnCanvasAnimatedControlDraw;
                AnimatedCanvas.Draw += OnCanvasAnimatedControlDraw;
            }
        }

        private void OnCanvasAnimatedControlCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            if (sender is not null)
            {
                sender.CreateResources -= OnCanvasAnimatedControlCreateResources;

                CreateRenderTarget(sender);

                Initialize?.Execute(parameter: default);
            }
        }

        void OnCanvasAnimatedControlDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            if (sender is null
                || args is null
                || _canvasRenderTarget is null)
            {
                return;
            }

            try
            {
                args.DrawingSession.DrawImage(_canvasRenderTarget);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        void EnqueueMessageToPipeline(Core.Messages.Base.SynchronousMessages.Message message)
        {
            if (!_drawingPipeline.TryEnqueue(message))
            {
                Logger.Information($"Cannot enqueue message: {message.GetType()}");
            }
        }

        void HandlePipelineMessage(DrawingMessageValue message)
        {
            if (message.IsDefault())
            {
                return;
            }

            switch (message.MessageType)
            {
                case DrawingMessageType.ClearCanvas: HandleClearCanvasMessage((ClearCanvasMessageValue)message); break;
                case DrawingMessageType.DrawPolygon: HandleConnectPointsCanvasMessage((ConnectPointsMessageValue)message); break;
                case DrawingMessageType.NoAction: break;
            }
        }

        void HandleClearCanvasMessage(ClearCanvasMessageValue message)
        {
            using (var drawingSession = _canvasRenderTarget.CreateDrawingSession())
            {
                drawingSession.Clear(message.Color.WindowsUIColor);
            }
        }

        void HandleConnectPointsCanvasMessage(ConnectPointsMessageValue message)
        {
            using (var builder = new CanvasPathBuilder(_canvasRenderTarget))
            {
                builder.BeginFigure(message.Points.First().ToVector2());

                for (int i = 1; i < message.Points.Count; i++)
                {
                    builder.AddLine(message.Points[i].ToVector2());
                }

                builder.EndFigure(CanvasFigureLoop.Open);

                using (var geometry = CanvasGeometry.CreatePath(builder))
                using (var drawingSession = _canvasRenderTarget.CreateDrawingSession())
                {
                    drawingSession.DrawGeometry(
                        geometry,
                        message.Color.WindowsUIColor,
                        message.Thickness);
                }
            }
        }

        async Task InitializeDrawingPipelineAsync()
        {
            _drawingPipeline.MessageReceived -= OnDrawingPipelineMessageReceived;
            _drawingPipeline.MessageReceived += OnDrawingPipelineMessageReceived;

            await Task.Run(_drawingPipeline.Initialize);
        }

        async Task DisposeDrawingPipelineAsync()
        {
            _drawingPipeline.MessageReceived -= OnDrawingPipelineMessageReceived;

            await _drawingPipeline.DisposeAsync();
        }

        private void OnDrawingPipelineMessageReceived(DrawingMessageValue value)
        {
            HandlePipelineMessage(value);
        }

        void CreateRenderTarget(CanvasAnimatedControl canvasAnimatedControl)
        {
            if (_canvasRenderTarget is null)
            {
                _canvasRenderTarget = CreateRenderTargetCore(canvasAnimatedControl);
            }
        }

        void DisposeRenderTarget()
        {
            if (_canvasRenderTarget is not null)
            {
                DisposeRenderTargetCore();

                _canvasRenderTarget = default!;
            }
        }

        static CanvasRenderTarget CreateRenderTargetCore(CanvasAnimatedControl canvas)
        {
            CanvasRenderTarget result = default!;

            try
            {
                result = new CanvasRenderTarget(
                                canvas,
                                (float)canvas.ActualWidth,
                                (float)canvas.ActualHeight,
                                DrawingConstants.STANDART_DPI);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }

            return result;
        }

        void DisposeRenderTargetCore()
        {
            try
            {
                _canvasRenderTarget.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
    }
}
