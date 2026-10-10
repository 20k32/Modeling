using Modeling.Core.Abstractions;
using Modeling.Core.Drawing;
using Modeling.Core.Enums;

namespace Modeling.Core.Messages.Parameters.Canvas.Drawing
{
    public sealed class ClearCanvasMessageParameter(DrawingColor color, bool clearBeforeRedraw, bool shouldApplyAliasing = false, DrawingAliasing drawingAliasing = DrawingAliasing.None, IObjectTree parent = default) : DrawingMessageParameter(clearBeforeRedraw, parent, shouldApplyAliasing, drawingAliasing)
    {
        public DrawingColor Color { get; init; } = color;
        public override bool IsDefault() => Color.IsDefault();
    }
}
