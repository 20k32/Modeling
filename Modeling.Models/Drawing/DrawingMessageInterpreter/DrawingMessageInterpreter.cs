using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Models.Drawing.DrawingMessageValues;

namespace Modeling.Models.Drawing.DrawingMessageInterpreter
{
    sealed class DrawingMessageInterpreter : IDrawingMessageInterpreter
    {
        static ClearCanvasMessageValue InterpretClearCanvasMessage(ClearCanvasMessage message) => new(message.Value);

        static ConnectPointsMessageValue InterpretConnectTwoPointsMessage(ConnectTwoPointsMessage message)
            => new(message.Value.Color, message.Value.Thickness, message.Value.ShouldClearBeforeRedraw, message.Value.BackgroundColor, message.Value.PointA, message.Value.PointB);

        static ConnectPointsMessageValue InterpretConnectPointsMessage(ConnectPointsMessage message)
            => new(message.Value.Color, message.Value.Thickness, message.Value.ShouldClearBeforeRedraw, message.Value.BackgroundColor, message.Value.Points);

        static TransformPointsMessageValue InterpretTransformPointsMessage(TransformPointsMessage message)
            => new(message.Value.Color, message.Value.Thickness, message.Value.TransformMatrix, message.Value.ShouldClearBeforeRedraw, message.Value.BackgroundColor, message.Value.Points);

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
            else if (message is TransformPointsMessage transformPointsMessage)
            {
                result = InterpretTransformPointsMessage(transformPointsMessage);
            }

            return result is not null;
        }
    }
}
