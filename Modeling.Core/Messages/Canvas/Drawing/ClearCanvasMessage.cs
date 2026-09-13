using Modeling.Core.Drawing;
using Modeling.Core.Messages.Base.SynchronousMessages;

namespace Modeling.Core.Messages.Canvas.Drawing
{
    public sealed class ClearCanvasMessage : ParametrizedMessage<DrawingColor>
    {
        public ClearCanvasMessage(object sender, DrawingColor value) : base(sender, value)
        { }
    }
}
