using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Logging;
using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Models.Drawing.DrawingMessageInterpreter;
using Modeling.Models.Drawing.DrawingMessageValues;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.Models.Drawing.DrawingPipeline
{
    sealed class DrawingPipeline : IDrawingPipeline
    {
        readonly IDrawingMessageInterpreter _messageInterpreter;
        public event Action<DrawingMessageValue> MessageReceived;

        public DrawingPipeline()
        {
            _messageInterpreter = Ioc.Default.GetService<IDrawingMessageInterpreter>();
        }

        public bool TryEnqueue(Message message)
        {
            var messageEnqueued = _messageInterpreter.TryInterpretMessage(message, out var interpretedValue);

            if (messageEnqueued)
            {
                MessageReceived?.Invoke(interpretedValue);
            }

            return messageEnqueued;
        }
    }
}
