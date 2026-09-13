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
            typeof(IAsyncRelayCommand),
            typeof(Win2DCanvas),
            new PropertyMetadata(default));

        public IAsyncRelayCommand Initialize
        {
            get { return (IAsyncRelayCommand)GetValue(InitializeProperty); }
            set { SetValue(InitializeProperty, value); }
        }

        public Win2DCanvas()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.Register<InitializeCanvasControlMessage>(this, OnWin2DCanvasInitializeCanvasControlMessage);
            WeakReferenceMessenger.Default.Register<DisposeCanvasControlMessage>(this, OnWin2DCanvasDisposeCanvasControlMessage);

            _drawingPipeline = Ioc.Default.GetRequiredService<IDrawingPipeline>();
        }

        void RegisterMessages()
        {
            WeakReferenceMessenger.Default.Register<InitializeDrawingSessionMessage>(this, OnWin2DCanvasInitializeDrawingSessionMessage);
            WeakReferenceMessenger.Default.Register<EndDrawingSessionMessage>(this, OnWin2DCanvasEndDrawingSessionMessage);
            WeakReferenceMessenger.Default.Register<ConnectTwoPointsMessage>(this, OnWin2DCanvasReceivedDrawingMessage);
            WeakReferenceMessenger.Default.Register<ClearCanvasMessage>(this, OnWin2DCanvasReceivedDrawingMessage);
            WeakReferenceMessenger.Default.Register<ConnectPointsMessage>(this, OnWin2DCanvasReceivedDrawingMessage);
            WeakReferenceMessenger.Default.Register<TransformPointsMessage>(this, OnWin2DCanvasReceivedDrawingMessage);
            WeakReferenceMessenger.Default.Register<ChangeRedrawStatusMessage>(this, OnWin2DCanvasChangeRedrawStatusMessage);
            WeakReferenceMessenger.Default.Register<ChangeCanvasRefreshRateMessage>(this, OnWin2DCanvasChangeCanvasRefreshRateMessage);
        }

        void UnregisterMessages()
        {
            WeakReferenceMessenger.Default.Unregister<InitializeDrawingSessionMessage>(this);
            WeakReferenceMessenger.Default.Unregister<EndDrawingSessionMessage>(this);
            WeakReferenceMessenger.Default.Unregister<ConnectTwoPointsMessage>(this);
            WeakReferenceMessenger.Default.Unregister<ClearCanvasMessage>(this);
            WeakReferenceMessenger.Default.Unregister<ConnectPointsMessage>(this);
            WeakReferenceMessenger.Default.Unregister<TransformPointsMessage>(this);
            WeakReferenceMessenger.Default.Unregister<ChangeRedrawStatusMessage>(this);
            WeakReferenceMessenger.Default.Unregister<ChangeCanvasRefreshRateMessage>(this);

        }

        void OnWin2DCanvasChangeCanvasRefreshRateMessage(object recipient, ChangeCanvasRefreshRateMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                AnimatedCanvas.TargetElapsedTime = message.Value.RefreshRate;
            }
        }

        void OnWin2DCanvasChangeRedrawStatusMessage(object recipient, ChangeRedrawStatusMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                AnimatedCanvas.Paused = message.Value.Paused;
            }
        }

        async void OnWin2DCanvasDisposeCanvasControlMessage(object recipient, DisposeCanvasControlMessage message)
        {
            if (_isControlInitialized && message.ApplyBasicMessageValidation(recipient))
            {
                _isControlInitialized = false;

                await DisposeDrawingPipelineAsync();

                UnregisterMessages();
            }
        }

        async void OnWin2DCanvasInitializeCanvasControlMessage(object recipient, InitializeCanvasControlMessage message)
        {
            if (!_isControlInitialized && message.ApplyBasicMessageValidation(recipient))
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

        void EnqueueMessageToPipeline(object recipient, Core.Messages.Base.SynchronousMessages.Message message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                EnqueueMessageToPipelineCore(message);
            }
        }

        void EnqueueMessageToPipelineCore(Core.Messages.Base.SynchronousMessages.Message message)
        {
            if (!_drawingPipeline.TryEnqueue(message))
            {
                Logger.Information($"Cannot enqueue message: {message.GetType()}");
            }
        }

        void OnWin2DCanvasReceivedDrawingMessage(object recipient, Core.Messages.Base.SynchronousMessages.Message message)
        {
            EnqueueMessageToPipeline(recipient, message);
        }

        void OnWin2DCanvasEndDrawingSessionMessage(object recipient, EndDrawingSessionMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                AnimatedCanvas.Draw -= OnCanvasAnimatedControlDraw;

                DisposeRenderTarget();
            }
        }

        void OnWin2DCanvasInitializeDrawingSessionMessage(object recipient, InitializeDrawingSessionMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                AnimatedCanvas.CreateResources -= OnCanvasAnimatedControlCreateResources;
                AnimatedCanvas.CreateResources += OnCanvasAnimatedControlCreateResources;

                AnimatedCanvas.Draw -= OnCanvasAnimatedControlDraw;
                AnimatedCanvas.Draw += OnCanvasAnimatedControlDraw;
            }
        }

        async void OnCanvasAnimatedControlCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            if (sender is not null)
            {
                sender.CreateResources -= OnCanvasAnimatedControlCreateResources;

                CreateRenderTarget(sender);

                await (Initialize?.ExecuteAsync(parameter: default) ?? Task.CompletedTask);
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

        void HandlePipelineMessage(DrawingMessageValue message)
        {
            if (message.IsDefault())
            {
                return;
            }

            switch (message.MessageType)
            {
                case DrawingMessageType.ClearAll: HandleClearCanvasMessage((ClearCanvasMessageValue)message); break;
                case DrawingMessageType.Draw: HandleConnectPointsCanvasMessage((ConnectPointsMessageValue)message); break;
                case DrawingMessageType.Transform: HandleTransformPointsCanvasMessage((TransformPointsMessageValue)message); break;
                case DrawingMessageType.NoAction: break;
            }
        }

        private void HandleTransformPointsCanvasMessage(TransformPointsMessageValue message)
        {
            using (var builder = new CanvasPathBuilder(_canvasRenderTarget))
            {
                var firstPoint = message.Points.First();
                var firstPointTransformed = message.Transform * firstPoint;

                builder.BeginFigure(firstPointTransformed.ToVector2());

                for (int i = 1; i < message.Points.Count; i++)
                {
                    var point = message.Points[i];
                    var transformedPoint = message.Transform * point;

                    builder.AddLine(transformedPoint.ToVector2());
                }

                builder.EndFigure(CanvasFigureLoop.Open);

                using (var geometry = CanvasGeometry.CreatePath(builder))
                using (var drawingSession = _canvasRenderTarget.CreateDrawingSession())
                {
                    if (message.ShouldClearBeforeRedraw)
                    {
                        drawingSession.Clear(message.BackgroundColor.WindowsUIColor);
                    }

                    drawingSession.DrawGeometry(
                        geometry,
                        message.Color.WindowsUIColor,
                        message.Thickness);
                }
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
                    if (message.ShouldClearBeforeRedraw)
                    {
                        drawingSession.Clear(message.BackgroundColor.WindowsUIColor);
                    }

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

        void OnDrawingPipelineMessageReceived(DrawingMessageValue value)
        {
            try
            {
                HandlePipelineMessage(value);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
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
