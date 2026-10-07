using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.CoreDelegates;
using Modeling.Core.Messages.Base.AsynchronousMessages;
using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Models.Drawing.DrawingMessageInterpreter;
using Modeling.Models.Drawing.DrawingMessageValues;
using System.Threading.Tasks;

namespace Modeling.Models.Drawing.DrawingPipeline
{
    sealed class DrawingPipeline : IDrawingPipeline
    {
        readonly IDrawingMessageInterpreter _messageInterpreter;
        public event AsyncActionEventHandler<DrawingMessageValue> MessageReceived;

        public DrawingPipeline()
        {
            _messageInterpreter = Ioc.Default.GetService<IDrawingMessageInterpreter>();
        }

        public async Task<bool> TryEnqueueAsync(AsyncMessage message)
        {
            var messageEnqueued = _messageInterpreter.TryInterpretMessage(message, out var interpretedValue);

            if (messageEnqueued && MessageReceived is not null)
            {
                await MessageReceived(interpretedValue);
            }

            return messageEnqueued;
        }
    }
}
