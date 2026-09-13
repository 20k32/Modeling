using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Models.Drawing.DrawingMessageValues;

namespace Modeling.Models.Drawing.DrawingMessageInterpreter
{
    sealed class DrawingMessageInterpreter : IDrawingMessageInterpreter
    {
        static ClearCanvasMessageValue InterpretClearCanvasMessage(ClearCanvasMessage message) => new(message.Value);

        static ConnectPointsMessageValue InterpretConnectTwoPointsMessage(ConnectTwoPointsMessage message)
            => new(message.Value.Color, message.Value.Thickness, message.Value.PointA, message.Value.PointB);

        static ConnectPointsMessageValue InterpretConnectPointsMessage(ConnectPointsMessage message)
            => new(message.Value.Color, message.Value.Thickness, message.Value.Points);

        public bool TryInterpretMessage(Message message, out DrawingMessageValue result)
        {
            result = default;

            if (message is ConnectTwoPointsMessage connectTwoPointsMessage)
            {
                result = InterpretConnectTwoPointsMessage(connectTwoPointsMessage);
            }
            else if (message is ClearCanvasMessage clearCanvasMessage)
            {
                result = InterpretClearCanvasMessage(clearCanvasMessage);
            }
            else if (message is ConnectPointsMessage connectPointsMessage)
            {
                result = InterpretConnectPointsMessage(connectPointsMessage);
            }

            return result is not null;
        }
    }
}
