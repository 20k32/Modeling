using Modeling.Core.Abstractions;
using Modeling.Core.Drawing;
using Modeling.Core.Enums;

namespace Modeling.Models.Drawing.DrawingMessageValues.Points
{
    public class DrawMessageValue(DrawingColor backgroundColor, bool shouldClearBeforeRedraw, bool shouldApplyAliasing, DrawingAliasing drawingAliasing) : IDefaultCheck
    {
        public DrawingColor BackgroundColor { get; init; } = backgroundColor;
        public bool ShouldClearBeforeRedraw { get; init; } = shouldClearBeforeRedraw;

        public bool ShouldApplyAliasing { get; init; } = shouldApplyAliasing;

        public DrawingAliasing DrawingAliasing { get; init; } = drawingAliasing;

        public bool IsDefault() => BackgroundColor.IsDefault();
    }
}
