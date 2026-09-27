using Modeling.Core.Abstractions;
using Modeling.Core.Drawing;

namespace Modeling.Core.Messages.Parameters.Canvas.Settings
{
    public sealed class UpdateDrawingsParameter(bool shouldUpdateCanvasSize, SizeSingle size, float pixelsPerCentimeter) : IDefaultCheck
    {
        public bool ShouldUpdateCanvasSize { get; init; } = shouldUpdateCanvasSize;
        public float PixelsPerCentimeter { get; init; } = pixelsPerCentimeter;
        public SizeSingle Size { get; init; } = size;
        public bool IsDefault() => false;
    }
}
