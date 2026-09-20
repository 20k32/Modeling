using Modeling.Core.Drawing;
using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Parameters.Canvas;

namespace Modeling.Core.Messages.Canvas.Drawing
{
    public sealed class ClearCanvasMessage(object sender, ClearCanvasMessageParameter value) : ParametrizedMessage<ClearCanvasMessageParameter>(sender, value)
    { }
}
