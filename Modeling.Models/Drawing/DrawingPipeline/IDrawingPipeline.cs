using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Models.Drawing.DrawingMessageValues;
using System;

namespace Modeling.Models.Drawing.DrawingPipeline
{
    public interface IDrawingPipeline : IAsyncDisposable
    {
        bool HasItems { get; }

        bool TryEnqueue(Message message);

        void Initialize();
        event Action<DrawingMessageValue> MessageReceived;
    }
}
