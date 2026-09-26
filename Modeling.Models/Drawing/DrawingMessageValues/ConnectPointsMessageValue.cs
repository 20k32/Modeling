using Modeling.Models.Drawing.DrawingMessageValues.Points;
using Modeling.Models.Enums;

namespace Modeling.Models.Drawing.DrawingMessageValues
{
    public sealed class ConnectPointsMessageValue : DrawingMessageValue
    {
        public ConnectPointsMessageValue() : base(DrawingMessageType.Draw)
        { }

        public override void AddDrawingParameter(DrawMessageValue value)
        {
            var actualPointMessageValue = (DrawPointsMessageValue)value;

            if (ValidateDrawingParameter(actualPointMessageValue))
            {
                pointMessages.Enqueue(actualPointMessageValue);
            }
        }

        public override bool IsDefault() => base.IsDefault();
    }
}
