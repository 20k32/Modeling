using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Parameters.Canvas.Settings;

namespace Modeling.Core.Messages.Canvas.Settings
{
    public sealed class ChangeRedrawStatusMessage(object sender, RedrawStatusParameter value)
        : ParametrizedMessage<RedrawStatusParameter>(sender, value)
    { }
}
