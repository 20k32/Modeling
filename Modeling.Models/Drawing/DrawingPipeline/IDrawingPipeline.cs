using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Models.Drawing.DrawingMessageValues;
using System;

namespace Modeling.Models.Drawing.DrawingPipeline
{
    public interface IDrawingPipeline
    {
        bool TryEnqueue(Message message);
        event Action<DrawingMessageValue> MessageReceived;
    }
}
