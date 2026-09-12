using Modeling.Core.Drawing;
using Modeling.Models.Enums;

namespace Modeling.Models.Drawing.DrawingMessageValues
{
    public sealed class ClearCanvasMessageValue : DrawingMessageValue
    {
        public ClearCanvasMessageValue(DrawingColor color) : base(color)
        {
            MessageType = DrawingMessageType.ClearCanvas;
        }
    }
}
