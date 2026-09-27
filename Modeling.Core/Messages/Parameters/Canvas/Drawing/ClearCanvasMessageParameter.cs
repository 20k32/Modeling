using Modeling.Core.Abstractions;
using Modeling.Core.Drawing;

namespace Modeling.Core.Messages.Parameters.Canvas.Drawing
{
    public sealed class ClearCanvasMessageParameter(DrawingColor color, bool clearBeforeRedraw, IObjectTree parent = default) : DrawingMessageParameter(clearBeforeRedraw, parent)
    {
        public DrawingColor Color { get; init; } = color;
        public override bool IsDefault() => Color.IsDefault();
    }
}
