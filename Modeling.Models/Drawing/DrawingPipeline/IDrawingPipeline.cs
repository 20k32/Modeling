using Modeling.Core.CoreDelegates;
using Modeling.Core.Messages.Base.AsynchronousMessages;
using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Models.Drawing.DrawingMessageValues;
using System;
using System.Threading.Tasks;

namespace Modeling.Models.Drawing.DrawingPipeline
{
    public interface IDrawingPipeline
    {
        Task<bool> TryEnqueueAsync(AsyncMessage message);
        event AsyncActionEventHandler<DrawingMessageValue> MessageReceived;
    }
}
