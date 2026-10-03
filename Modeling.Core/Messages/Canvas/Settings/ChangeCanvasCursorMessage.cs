using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Parameters.Canvas.Settings;

namespace Modeling.Core.Messages.Canvas.Settings
{
    public sealed class ChangeCanvasCursorMessage(object sender, CanvasCursorMessageParameter value)
        : ParametrizedMessage<CanvasCursorMessageParameter>(sender, value)
    { }
}
