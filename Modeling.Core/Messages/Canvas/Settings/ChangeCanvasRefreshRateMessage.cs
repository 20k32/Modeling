using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Parameters.Canvas;

namespace Modeling.Core.Messages.Canvas.Settings
{
    public sealed class ChangeCanvasRefreshRateMessage(object sender, RefreshRateParameter value)
        : ParametrizedMessage<RefreshRateParameter>(sender, value)
    { }
}
