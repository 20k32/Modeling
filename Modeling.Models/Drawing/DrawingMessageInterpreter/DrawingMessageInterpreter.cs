using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Models.Drawing.DrawingMessageValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeling.Models.Drawing.DrawingMessageInterpreter
{
    sealed class DrawingMessageInterpreter : IDrawingMessageInterpreter
    {
        static ClearCanvasMessageValue InterpretClearCanvasMessage(ClearCanvasMessage message) => new(message.Value);

        static ConnectPointsMessageValue InterpretConnectTwoPointsMessage(ConnectTwoPointsMessage message)
            => new(message.Value.Color, message.Value.Thickness, message.Value.PointA, message.Value.PointB);

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

            return result is not null;
        }
    }
}
