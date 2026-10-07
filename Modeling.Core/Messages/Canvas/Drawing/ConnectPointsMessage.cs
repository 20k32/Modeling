using Modeling.Core.Messages.Base.AsynchronousMessages;
using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Parameters.Canvas.Drawing;

namespace Modeling.Core.Messages.Canvas.Drawing
{
    public sealed class ConnectPointsMessage(object sender, PointListMessageParameter value) : ParametrizedAsyncMessage<PointListMessageParameter>(sender, value)
    { }
}
