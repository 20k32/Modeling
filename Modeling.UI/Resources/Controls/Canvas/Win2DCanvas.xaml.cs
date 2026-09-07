using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Modeling.Core.Constants;
using Modeling.Core.Drawing;
using Modeling.Core.Extensions;
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Core.Messages.Canvas.Settings;
using System;
using System.Collections.Concurrent;


namespace Modeling.UI.Resources.Controls.Canvas
{
    public sealed partial class Win2DCanvas : UserControl
    {
        private readonly ConcurrentQueue<PointSingle> pipeline;

        private PointSingle previousPoint = DrawingConstants.INVALID_POINT;
        private bool _isControlInitialized;

        public Win2DCanvas()
        {
            InitializeComponent();

            pipeline = new();

            WeakReferenceMessenger.Default.Register<InitializeCanvasControlMessage>(this, OnWin2DCanvasInitializeCanvasControlMessage);
            WeakReferenceMessenger.Default.Register<DisposeCanvasControlMessage>(this, OnWin2DCanvasDisposeCanvasControlMessage);
        }

        private void OnWin2DCanvasDisposeCanvasControlMessage(object recipient, DisposeCanvasControlMessage message)
        {
            if (_isControlInitialized && message.ApplyBaseMessageValidation(recipient))
            {
                WeakReferenceMessenger.Default.UnregisterAll(this);
            }
        }

        private void OnWin2DCanvasInitializeCanvasControlMessage(object recipient, InitializeCanvasControlMessage message)
        {
            if (!_isControlInitialized && message.ApplyBaseMessageValidation(recipient))
            {
                _isControlInitialized = true;
                RegisterMessages();
            }
        }

        private void RegisterMessages()
        {
            WeakReferenceMessenger.Default.Register<InitializeDrawingSessionMessage>(this, OnWin2DCanvasInitializeDrawingSessionMessage);
            WeakReferenceMessenger.Default.Register<EndDrawingSessionMessage>(this, OnWin2DCanvasEndDrawingSessionMessage);
            WeakReferenceMessenger.Default.Register<ConnectTwoPointsMessage>(this, OnWin2DCanvasConnectTwoPointsMessage);
        }

        private void OnWin2DCanvasConnectTwoPointsMessage(object recipient, ConnectTwoPointsMessage message)
        {
            if (message.ApplyBaseMessageValidation(recipient))
            {
                pipeline.Enqueue(message.Value.PointA);
                pipeline.Enqueue(message.Value.PointB);
            }
        }

        private void OnWin2DCanvasEndDrawingSessionMessage(object recipient, EndDrawingSessionMessage message)
        {
            if (message.ApplyBaseMessageValidation(recipient))
            {
                AnimatedCanvas.Draw -= OnCanvasAnimatedControlDraw;
            }
        }

        private void OnWin2DCanvasInitializeDrawingSessionMessage(object recipient, InitializeDrawingSessionMessage message)
        {
            if (message.ApplyBaseMessageValidation(recipient))
            {
                AnimatedCanvas.Draw -= OnCanvasAnimatedControlDraw;
                AnimatedCanvas.Draw += OnCanvasAnimatedControlDraw;
            }
        }

        private void OnCanvasAnimatedControlDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            if (pipeline.IsEmpty)
            {
                return;
            }

            while (!pipeline.IsEmpty)
            {
                if (pipeline.TryDequeue(out var point))
                {
                    if (previousPoint != DrawingConstants.INVALID_POINT)
                    {
                        args.DrawingSession.DrawLine(previousPoint.ToVector2(), point.ToVector2(), Colors.Black);
                    }

                    previousPoint = point;
                }
            }
        }
    }
}
