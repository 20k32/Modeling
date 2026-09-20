using Modeling.Core.Messages.Base.SynchronousMessages;
using Modeling.Core.Messages.Canvas.Drawing;
using Modeling.Models.Drawing.DrawingMessageValues;
using Modeling.Core.Extensions;
using Modeling.Models.Drawing.DrawingMessageValues.Points;
using Modeling.Core.Messages.Parameters.Canvas;

namespace Modeling.Models.Drawing.DrawingMessageInterpreter
{
    sealed class DrawingMessageInterpreter : IDrawingMessageInterpreter
    {
        static ClearCanvasMessageValue InterpretClearCanvasMessage(ClearCanvasMessage message)
        {
            var result = new ClearCanvasMessageValue();

            foreach (var item in message.Value.TraverseFromParent())
            {
                var connectPointsParameter = item as ClearCanvasMessageParameter;

                var drawingParameter = new DrawMessageValue(
                    connectPointsParameter.Color,
                    connectPointsParameter.ClearBeforeRedraw);

                result.AddDrawingParameter(drawingParameter);
            }

            return result;
        }

        static ConnectPointsMessageValue InterpretConnectPointsMessage(ConnectPointsMessage message)
        {
            var result = new ConnectPointsMessageValue();

            foreach (var item in message.Value.TraverseFromParent())
            {
                var connectPointsParameter = item as PointListMessageParameter;

                var drawingParameter = new DrawPointsMessageValue(
                    connectPointsParameter.Color,
                    connectPointsParameter.Thickness,
                    connectPointsParameter.ShouldClearBeforeRedraw,
                    connectPointsParameter.BackgroundColor,
                    connectPointsParameter.Points);

                result.AddDrawingParameter(drawingParameter);
            }

            return result;
        }

        static TransformPointsMessageValue InterpretTransformPointsMessage(TransformPointsMessage message)
        {
            var result = new TransformPointsMessageValue();

            foreach (var item in message.Value.TraverseFromParent())
            {
                var connectPointsParameter = item as PointListTransformMessageParameter;

                var drawingParameter = new DrawTransformedPointsMessageValue(
                    connectPointsParameter.Color,
                    connectPointsParameter.Thickness,
                    connectPointsParameter.ShouldClearBeforeRedraw,
                    connectPointsParameter.BackgroundColor,
                    connectPointsParameter.TransformMatrix,
                    connectPointsParameter.Points);

                result.AddDrawingParameter(drawingParameter);
            }

            return result;
        }

        public bool TryInterpretMessage(Message message, out DrawingMessageValue result)
        {
            result = default;

            if (message is ClearCanvasMessage clearCanvasMessage)
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
