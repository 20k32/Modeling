using Modeling.Core.Drawing;
using Modeling.Models.Drawing.DrawingMessageValues.Points;
using Modeling.Models.Enums;

namespace Modeling.Models.Drawing.DrawingMessageValues
{
    public class TransformPointsMessageValue : DrawingMessageValue
    {
        public override bool IsDefault() => base.IsDefault();

        public TransformPointsMessageValue() : base(DrawingMessageType.Transform)
        { }

        public override void AddDrawingParameter(DrawMessageValue value)
        {
            var actualPointMessageValue = (Points.DrawTransformedPointsMessageValue)value;

            if (ValidateDrawingParameter(actualPointMessageValue))
            {
                pointMessages.Enqueue(actualPointMessageValue);
            }
        }
    }
}
