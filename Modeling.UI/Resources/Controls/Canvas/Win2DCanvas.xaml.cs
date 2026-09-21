using ABI.Microsoft.UI.Xaml.Media;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Core.Messages.Canvas.Settings;
using Modeling.Models.Drawing.DrawingMessageValues;
using Modeling.Models.Drawing.DrawingMessageValues.Points;
using Modeling.Models.Drawing.DrawingPipeline;
using Modeling.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;


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

        public static readonly DependencyProperty PointerMovedCommandProperty =
            DependencyProperty.Register(nameof(PointerMovedCommand),
            typeof(IRelayCommand<PointSingle>),
            typeof(Win2DCanvas),
            new PropertyMetadata(default));

        public IRelayCommand<PointSingle> PointerMovedCommand
        {
            get { return (IRelayCommand<PointSingle>)GetValue(PointerMovedCommandProperty); }
            set { SetValue(PointerMovedCommandProperty, value); }
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

                AnimatedCanvas.PointerWheelChanged -= OnAnimatedCanvasPointerWheelChanged;
                AnimatedCanvas.PointerMoved -= OnAnimatedCanvasPointerMoved;
            }
        }

        async void OnWin2DCanvasInitializeCanvasControlMessage(object recipient, InitializeCanvasControlMessage message)
        {
            if (!_isControlInitialized && message.ApplyBasicMessageValidation(recipient))
            {
                Logger.InitializedInformation(nameof(Win2DCanvas));

                _isControlInitialized = true;

                RegisterMessages();

                AnimatedCanvas.PointerWheelChanged += OnAnimatedCanvasPointerWheelChanged;
                AnimatedCanvas.PointerMoved += OnAnimatedCanvasPointerMoved;

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
                
                ScrollToCenter();
            }
        }

        void ScrollToCenter()
        {
            var canvasHorizontalCenter = (AnimatedCanvas.Width - CanvasScrollViewer.ActualWidth) / 2;
            var canvasVerticalCenter = (AnimatedCanvas.Height - CanvasScrollViewer.ActualHeight) / 2;

            CanvasScrollViewer.ScrollToHorizontalOffset(Math.Max(0, canvasHorizontalCenter));
            CanvasScrollViewer.ScrollToVerticalOffset(Math.Max(0, canvasVerticalCenter));
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

        void HandleClearCanvasMessage(ClearCanvasMessageValue message)
        {
            using (var drawingSession = _canvasRenderTarget.CreateDrawingSession())
            {
                foreach (var drawingParameter in message.DrawingParameters.Where(parameter => parameter.ShouldClearBeforeRedraw))
                {
                    drawingSession.Clear(drawingParameter.BackgroundColor.WindowsUIColor);
                }
            }
        }

        static void DrawFigure(CanvasPathBuilder builder, IReadOnlyList<PointSingle> points, bool applyTransform, Matrix3x3Single transform)
        {
            bool figureStarted = false;

            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];

                if (float.IsNaN(point.X) || float.IsNaN(point.Y))
                {
                    if (figureStarted)
                    {
                        builder.EndFigure(CanvasFigureLoop.Open);
                        figureStarted = false;
                    }

                    continue;
                }

                var transformedPoint = applyTransform ? transform * point : point;

                if (!figureStarted)
                {
                    builder.BeginFigure(transformedPoint.ToVector2());
                    figureStarted = true;
                }
                else
                {
                    builder.AddLine(transformedPoint.ToVector2());
                }
            }

            if (figureStarted)
            {
                builder.EndFigure(CanvasFigureLoop.Open);
            }
        }

        void HandleConnectPointsCanvasMessage(ConnectPointsMessageValue message)
        {
            using (var drawingSession = _canvasRenderTarget.CreateDrawingSession())
            {
                foreach (var drawingParameter in message.DrawingParameters.OfType<DrawPointsMessageValue>())
                {
                    HandleConnectPointsCanvasMessageCore(drawingSession, drawingParameter);
                }
            }
        }

        void HandleConnectPointsCanvasMessageCore(CanvasDrawingSession drawingSession, DrawPointsMessageValue message)
        {
            if (message.ShouldClearBeforeRedraw)
            {
                drawingSession.Clear(message.BackgroundColor.WindowsUIColor);
            }

            using (var builder = new CanvasPathBuilder(_canvasRenderTarget))
            {
                DrawFigure(builder, message.Points, applyTransform: false, DrawingConstants.NON_TRANSFORM_MATRIX);

                using (var geometry = CanvasGeometry.CreatePath(builder))
                using (var strokeStyle = new CanvasStrokeStyle
                {
                    LineJoin = CanvasLineJoin.Round,
                    StartCap = CanvasCapStyle.Round,
                    EndCap = CanvasCapStyle.Round
                })
                {
                    if (message.ShouldFillGeometry)
                    {
                        drawingSession.FillGeometry(
                        geometry,
                        message.FillColor.WindowsUIColor);
                    }

                    drawingSession.DrawGeometry(
                        geometry,
                        message.Color.WindowsUIColor,
                        message.Thickness,
                        strokeStyle);
                }
            }
        }

        void HandleTransformPointsCanvasMessage(TransformPointsMessageValue message)
        {
            using (var drawingSession = _canvasRenderTarget.CreateDrawingSession())
            {
                foreach (var drawingParameter in message.DrawingParameters.OfType<DrawTransformedPointsMessageValue>())
                {
                    HandleTransformPointsCanvasMessageCore(drawingSession, drawingParameter);
                }
            }
        }

        void HandleTransformPointsCanvasMessageCore(CanvasDrawingSession drawingSession, DrawTransformedPointsMessageValue message)
        {
            var shouldApplyTransform = message.Transform != default
                && message.Transform != DrawingConstants.NON_TRANSFORM_MATRIX;

            using (var builder = new CanvasPathBuilder(_canvasRenderTarget))
            {
                DrawFigure(builder, message.Points, shouldApplyTransform, message.Transform);

                using (var geometry = CanvasGeometry.CreatePath(builder))
                using (var strokeStyle = new CanvasStrokeStyle
                {
                    LineJoin = CanvasLineJoin.Round,
                    StartCap = CanvasCapStyle.Round,
                    EndCap = CanvasCapStyle.Round
                })
                {
                    if (message.ShouldClearBeforeRedraw)
                    {
                        drawingSession.Clear(message.BackgroundColor.WindowsUIColor);
                    }

                    if (message.ShouldFillGeometry)
                    {
                        drawingSession.FillGeometry(
                        geometry,
                        message.FillColor.WindowsUIColor);
                    }
                    
                    drawingSession.DrawGeometry(
                        geometry,
                        message.Color.WindowsUIColor,
                        message.Thickness,
                        strokeStyle);
                }
            }
        }

        async Task InitializeDrawingPipelineAsync()
        {
            _drawingPipeline.MessageReceived -= OnDrawingPipelineMessageReceived;
            _drawingPipeline.MessageReceived += OnDrawingPipelineMessageReceived;

            await Task.Run(_drawingPipeline.Initialize).ConfigureAwait(false);
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

        private void OnAnimatedCanvasPointerWheelChanged(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var scrollViewer = CanvasScrollViewer;

            var point = e.GetCurrentPoint(scrollViewer);
            var delta = point.Properties.MouseWheelDelta;

            e.Handled = true;

            var offsetX = default(double?);
            var offsetY = default(double?);

            if (InputKeyboardSource.GetKeyStateForCurrentThread(
                    VirtualKey.Shift)
                .HasFlag(CoreVirtualKeyStates.Down))
            {
                var offset = scrollViewer.HorizontalOffset;

                offsetX = offset - delta;
            }
            else
            {
                var offset = scrollViewer.VerticalOffset;

                offsetY = offset - delta;
            }

            scrollViewer.ChangeView(
                    offsetX,
                    offsetY,
                    null,
                    disableAnimation: false);
        }

        private void OnAnimatedCanvasPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (sender is CanvasAnimatedControl canvasControl
                && e is not null
                && PointerMovedCommand is not null)
            {
                var pointerPoint = e.GetCurrentPoint(canvasControl);
                var position = pointerPoint.Position;

                PointerMovedCommand.Execute(new PointSingle((float)position.X, (float)position.Y));
            }
        }
    }
}
