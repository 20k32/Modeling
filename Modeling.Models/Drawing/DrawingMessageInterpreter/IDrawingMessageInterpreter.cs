using Modeling.Core.Messages.Base.AsynchronousMessages;
using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Models.Drawing.DrawingMessageValues;

namespace Modeling.Models.Drawing.DrawingMessageInterpreter
{
    public interface IDrawingMessageInterpreter
    {
        bool TryInterpretMessage(AsyncMessage message, out DrawingMessageValue result);
    }
}
