using Modeling.Models.Drawing.DrawingMessageValues.Points;
using Modeling.Models.Enums;

namespace Modeling.Models.Drawing.DrawingMessageValues
{
    public sealed class ClearCanvasMessageValue : DrawingMessageValue
    {
        public override bool IsDefault() => base.IsDefault();

        public ClearCanvasMessageValue() : base(DrawingMessageType.ClearAll)
        { }

        public override void AddDrawingParameter(DrawMessageValue value)
        {
            if (ValidateDrawingParameter(value))
            {
                pointMessages.Enqueue(value);
            }
        }
    }
}
