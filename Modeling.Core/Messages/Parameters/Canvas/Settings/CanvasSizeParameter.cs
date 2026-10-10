using Modeling.Core.Abstractions;
using Modeling.Core.Drawing;

namespace Modeling.Core.Messages.Parameters.Canvas.Settings
{
    public sealed class UpdateDrawingsParameter(bool shouldUpdateCanvasSize, bool shouldRecreateFigure, bool shouldUpdateRefreshRate, bool applyAntiAliasing, SizeSingle size, float pixelsPerCentimeter, int refreshRate) : IDefaultCheck
    {
        public bool ShouldUpdateRefreshRate { get; init; } = shouldUpdateRefreshRate;
        public int RefreshRate { get; init; } = refreshRate;
        public bool ShouldRecreateFigure { get; init; } = shouldRecreateFigure;
        public bool ShouldUpdateCanvasSize { get; init; } = shouldUpdateCanvasSize;
        public bool ShouldApplyAntiAliasing { get; init; } = applyAntiAliasing;
        public float PixelsPerCentimeter { get; init; } = pixelsPerCentimeter;
        public SizeSingle Size { get; init; } = size;
        public bool IsDefault() => false;
    }
}
