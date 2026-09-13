using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Parameters.Canvas;

namespace Modeling.Core.Messages.Canvas.Drawing
{
    public sealed class TransformPointsMessage(object sender, PointListTransformMessageParameter value) : ParametrizedMessage<PointListTransformMessageParameter>(sender, value)
    { }
}
