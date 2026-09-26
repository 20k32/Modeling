using Modeling.Core.Abstractions;
using Modeling.Core.Drawing;

namespace Modeling.Models.Drawing.DrawingMessageValues.Points
{
    public class DrawMessageValue(DrawingColor backgroundColor, bool shouldClearBeforeRedraw) : IDefaultCheck
    {
        public DrawingColor BackgroundColor { get; init; } = backgroundColor;
        public bool ShouldClearBeforeRedraw { get; init; } = shouldClearBeforeRedraw;

        public bool IsDefault() => BackgroundColor.IsDefault();
    }
}
