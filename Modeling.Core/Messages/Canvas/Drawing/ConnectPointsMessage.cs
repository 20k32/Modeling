using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Parameters.Canvas;

namespace Modeling.Core.Messages.Canvas.Drawing
{
    public sealed class ConnectPointsMessage : ParametrizedMessage<PointListMessageParameter>
    {
        public ConnectPointsMessage(object sender, PointListMessageParameter value) : base(sender, value)
        { }
    }
}
