using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Modeling.Core.Abstractions.Collections;
using Modeling.Core.Constants;
using Modeling.Core.CoreDelegates;
using Modeling.Core.Drawing;
using Modeling.Core.Enums;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Base.AsynchronousMessages;
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Core.Messages.Canvas.Settings;
using Modeling.Core.Miscellaneous;
using Modeling.Models.Drawing.DrawingMessageValues;
using Modeling.Models.Drawing.DrawingMessageValues.Points;
using Modeling.Models.Drawing.DrawingPipeline;
using Modeling.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;


namespace Modeling.UI.Resources.Controls.Canvas
{
    public sealed partial class Win2DCanvas : UserControl
    {
        readonly SemaphoreSlim _processingPointsLock;

        readonly Dictionary<MouseCursor, InputSystemCursor> _cursors;

        readonly IDrawingPipeline _drawingPipeline;

        CancellationTokenSource _processingPointsSource;

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

        public static readonly DependencyProperty PointerPressedCommandProperty =
            DependencyProperty.Register(nameof(PointerPressedCommand),
            typeof(IRelayCommand<PointSingle>),
            typeof(Win2DCanvas),
            new PropertyMetadata(default));

        public IRelayCommand<PointSingle> PointerPressedCommand
        {
            get { return (IRelayCommand<PointSingle>)GetValue(PointerPressedCommandProperty); }
            set { SetValue(PointerPressedCommandProperty, value); }
        }

        public static readonly DependencyProperty PointerReleasedCommandProperty =
            DependencyProperty.Register(nameof(PointerReleasedCommand),
            typeof(IRelayCommand<PointSingle>),
            typeof(Win2DCanvas),
            new PropertyMetadata(default));

        public IRelayCommand<PointSingle> PointerReleasedCommand
        {
            get { return (IRelayCommand<PointSingle>)GetValue(PointerReleasedCommandProperty); }
            set { SetValue(PointerReleasedCommandProperty, value); }
        }

        public Win2DCanvas()
        {
            InitializeComponent();

            _processingPointsLock = new SemaphoreSlim(1, 1);

            WeakReferenceMessenger.Default.Register<InitializeCanvasControlMessage>(this, OnWin2DCanvasInitializeCanvasControlMessage);
            WeakReferenceMessenger.Default.Register<DisposeCanvasControlMessage>(this, OnWin2DCanvasDisposeCanvasControlMessage);

            _drawingPipeline = Ioc.Default.GetRequiredService<IDrawingPipeline>();

            _cursors = new Dictionary<MouseCursor, InputSystemCursor>()
            {
                { MouseCursor.Default, InputSystemCursor.Create(InputSystemCursorShape.Arrow) },
                { MouseCursor.Move, InputSystemCursor.Create(InputSystemCursorShape.SizeAll) },
                { MouseCursor.Finger, InputSystemCursor.Create(InputSystemCursorShape.Hand) }
            };
        }

        async Task TryExecuteActionAsync(ActionEventHandler<CancellationToken> action)
        {
            try
            {
                await _processingPointsSource.TryCancelAsync();

                using (var processingPointsSource = new CancellationTokenSource())
                {
                    var token = processingPointsSource.Token;

                    _processingPointsSource = processingPointsSource;

                    try
                    {
                        await _processingPointsLock.WaitAsync(token);

                        action(token);
                    }
                    finally
                    {
                        _processingPointsLock.ReleaseSafe();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is not OperationCanceledException
                    && ex is not ObjectDisposedException)
                {
                    Logger.Exception(ex);
                }
            }
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
            WeakReferenceMessenger.Default.Register<ChangeCanvasSizeMessage>(this, OnWin2DCanvasChangeCanvasSizeMessage);
            WeakReferenceMessenger.Default.Register<ChangeCanvasCursorMessage>(this, OnWin2DCanvasChangeCanvasCursorMessage);
        }

        void OnWin2DCanvasChangeCanvasSizeMessage(object recipient, ChangeCanvasSizeMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                DisposeRenderTarget();
                CreateRenderTarget(AnimatedCanvas, message.Value.Size);
            }
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
            WeakReferenceMessenger.Default.Unregister<ChangeCanvasSizeMessage>(this);
            WeakReferenceMessenger.Default.Unregister<ChangeCanvasCursorMessage>(this);
        }

        void OnWin2DCanvasChangeCanvasCursorMessage(object recipient, ChangeCanvasCursorMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient)
                && _cursors.TryGetValue(message.Value.MouseCursor, out var inputCursor)
                && ProtectedCursor != inputCursor)
            {
                ProtectedCursor = inputCursor;
            }
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

                DisposeDrawingPipeline();

                UnregisterMessages();

                AnimatedCanvas.PointerWheelChanged -= OnAnimatedCanvasPointerWheelChanged;
                AnimatedCanvas.PointerMoved -= OnAnimatedCanvasPointerMoved;
                AnimatedCanvas.PointerPressed -= OnAnimatedCanvasPointerPressed;
                AnimatedCanvas.PointerReleased -= OnAnimatedCanvasPointerReleased;
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
                AnimatedCanvas.PointerPressed += OnAnimatedCanvasPointerPressed;
                AnimatedCanvas.PointerReleased += OnAnimatedCanvasPointerReleased;

                InitializeDrawingPipeline();
            }
            else
            {
                Logger.Information("Canvas control already initialized");
            }
        }

        async Task<Unit> EnqueueMessageToPipelineAsync(object recipient, AsyncMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                await EnqueueMessageToPipelineCoreAsync(message);
            }

            return Unit.Default;
        }

        async Task EnqueueMessageToPipelineCoreAsync(AsyncMessage message)
        {
            if (!await _drawingPipeline.TryEnqueueAsync(message))
            {
                Logger.Information($"Cannot enqueue message: {message.GetType()}");
            }
        }

        void OnWin2DCanvasReceivedDrawingMessage(object recipient, AsyncMessage message)
        {
            message.Reply(EnqueueMessageToPipelineAsync(recipient, message));
        }

        void OnWin2DCanvasEndDrawingSessionMessage(object recipient, EndDrawingSessionMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                EndDrawingSessionCore();
            }
        }

        void EndDrawingSessionCore()
        {
            AnimatedCanvas.Draw -= OnCanvasAnimatedControlDraw;

            DisposeRenderTarget();
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

        async Task HandlePipelineMessageAsync(DrawingMessageValue message)
        {
            if (message.IsDefault())
            {
                return;
            }

            switch (message.MessageType)
            {
                case DrawingMessageType.ClearAll: await HandleClearCanvasMessageAsync((ClearCanvasMessageValue)message); break;
                case DrawingMessageType.Draw: await HandleConnectPointsCanvasMessageAsync((ConnectPointsMessageValue)message); break;
                case DrawingMessageType.Transform: await HandleTransformPointsCanvasMessageAsync((TransformPointsMessageValue)message); break;
                case DrawingMessageType.NoAction: break;
            }
        }

        async Task HandleClearCanvasMessageAsync(ClearCanvasMessageValue message)
        {
            await TryExecuteActionAsync((token) =>
            {
                using var drawingSession = _canvasRenderTarget.CreateDrawingSession();

                foreach (var drawingParameter in message.DrawingParameters.Where(parameter => parameter.ShouldClearBeforeRedraw))
                {
                    token.ThrowIfCancellationRequested();

                    drawingSession.Clear(drawingParameter.BackgroundColor.WindowsUIColor);
                }
            });
        }

        static void DrawFigure(CanvasPathBuilder builder, IBlockingCollection<PointSingle> points, bool applyTransform, Matrix3x3Single transform, CancellationToken token = default)
        {
            bool figureStarted = false;

            foreach (var point in points)
            {
                token.ThrowIfCancellationRequested();

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

            token.ThrowIfCancellationRequested();

            if (figureStarted)
            {
                builder.EndFigure(CanvasFigureLoop.Open);
            }
        }

        async Task HandleConnectPointsCanvasMessageAsync(ConnectPointsMessageValue message)
        {
            await TryExecuteActionAsync((token) =>
            {
                using var drawingSession = _canvasRenderTarget.CreateDrawingSession();
                foreach (var drawingParameter in message.DrawingParameters.OfType<DrawPointsMessageValue>())
                {
                    token.ThrowIfCancellationRequested();

                    HandleConnectPointsCanvasMessageCore(drawingSession, drawingParameter, token);
                }
            });
        }

        void HandleConnectPointsCanvasMessageCore(CanvasDrawingSession drawingSession, DrawPointsMessageValue message, CancellationToken token)
        {
            if (message.ShouldClearBeforeRedraw)
            {
                drawingSession.Clear(message.BackgroundColor.WindowsUIColor);
            }

            token.ThrowIfCancellationRequested();

            using var builder = new CanvasPathBuilder(_canvasRenderTarget);
            DrawFigure(builder, message.Points, applyTransform: false, DrawingConstants.NON_TRANSFORM_MATRIX, token);

            using var geometry = CanvasGeometry.CreatePath(builder);
            using var strokeStyle = new CanvasStrokeStyle
            {
                LineJoin = CanvasLineJoin.Round,
                StartCap = CanvasCapStyle.Round,
                EndCap = CanvasCapStyle.Round
            };

            token.ThrowIfCancellationRequested();

            if (message.ShouldFillGeometry)
            {
                drawingSession.FillGeometry(
                geometry,
                message.FillColor.WindowsUIColor);
            }

            token.ThrowIfCancellationRequested();

            drawingSession.DrawGeometry(
                geometry,
                message.Color.WindowsUIColor,
                message.Thickness,
                strokeStyle);
        }

        async Task HandleTransformPointsCanvasMessageAsync(TransformPointsMessageValue message)
        {
            await TryExecuteActionAsync((token) =>
            {
                using var drawingSession = _canvasRenderTarget.CreateDrawingSession();
                foreach (var drawingParameter in message.DrawingParameters.OfType<DrawTransformedPointsMessageValue>())
                {
                    token.ThrowIfCancellationRequested();

                    HandleTransformPointsCanvasMessageCore(drawingSession, drawingParameter, token);
                }
            });
        }

        void HandleTransformPointsCanvasMessageCore(CanvasDrawingSession drawingSession, DrawTransformedPointsMessageValue message, CancellationToken token)
        {
            var shouldApplyTransform = message.Transform != default
                && message.Transform != DrawingConstants.NON_TRANSFORM_MATRIX;

            using var builder = new CanvasPathBuilder(_canvasRenderTarget);
            DrawFigure(builder, message.Points, shouldApplyTransform, message.Transform, token);

            using var geometry = CanvasGeometry.CreatePath(builder);
            using var strokeStyle = new CanvasStrokeStyle
            {
                LineJoin = CanvasLineJoin.Round,
                StartCap = CanvasCapStyle.Round,
                EndCap = CanvasCapStyle.Round
            };

            token.ThrowIfCancellationRequested();

            if (message.ShouldClearBeforeRedraw)
            {
                drawingSession.Clear(message.BackgroundColor.WindowsUIColor);
            }

            token.ThrowIfCancellationRequested();

            if (message.ShouldFillGeometry)
            {
                drawingSession.FillGeometry(
                geometry,
                message.FillColor.WindowsUIColor);
            }

            token.ThrowIfCancellationRequested();

            drawingSession.DrawGeometry(
                geometry,
                message.Color.WindowsUIColor,
                message.Thickness,
                strokeStyle);
        }

        void InitializeDrawingPipeline()
        {
            _drawingPipeline.MessageReceived -= OnDrawingPipelineMessageReceived;
            _drawingPipeline.MessageReceived += OnDrawingPipelineMessageReceived;
        }

        void DisposeDrawingPipeline()
        {
            _drawingPipeline.MessageReceived -= OnDrawingPipelineMessageReceived;
        }

        async Task OnDrawingPipelineMessageReceived(DrawingMessageValue value)
        {
            try
            {
                await HandlePipelineMessageAsync(value);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        void CreateRenderTarget(CanvasAnimatedControl canvasAnimatedControl, SizeSingle size)
        {
            if (_canvasRenderTarget is null)
            {
                _canvasRenderTarget = CreateRenderTargetCore(canvasAnimatedControl, size);
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

        static CanvasRenderTarget CreateRenderTargetCore(CanvasAnimatedControl canvas, SizeSingle size)
        {
            CanvasRenderTarget result = default!;

            try
            {
                result = new CanvasRenderTarget(
                                canvas,
                                size.Width,
                                size.Height,
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

        void OnAnimatedCanvasPointerWheelChanged(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
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

        void OnAnimatedCanvasPointerMoved(object sender, PointerRoutedEventArgs e)
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

        void OnAnimatedCanvasPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (sender is CanvasAnimatedControl canvasControl
                && e is not null
                && PointerPressedCommand is not null)
            {
                var pointerPoint = e.GetCurrentPoint(canvasControl);
                var position = pointerPoint.Position;

                PointerPressedCommand.Execute(new PointSingle((float)position.X, (float)position.Y));
            }
        }

        void OnAnimatedCanvasPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (sender is CanvasAnimatedControl canvasControl
                && e is not null
                && PointerReleasedCommand is not null)
            {
                var pointerPoint = e.GetCurrentPoint(canvasControl);
                var position = pointerPoint.Position;

                PointerReleasedCommand.Execute(new PointSingle((float)position.X, (float)position.Y));
            }
        }
    }
}
