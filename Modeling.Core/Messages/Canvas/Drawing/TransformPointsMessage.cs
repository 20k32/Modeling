using Modeling.Core.Messages.Base.AsynchronousMessages;
using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Parameters.Canvas.Drawing;

namespace Modeling.Core.Messages.Canvas.Drawing
{
    public sealed class TransformPointsMessage(object sender, PointListTransformMessageParameter value) : ParametrizedAsyncMessage<PointListTransformMessageParameter>(sender, value)
    { }
}
