using Modeling.Core.Abstractions;
using Modeling.Core.Drawing;

namespace Modeling.Core.Messages.Parameters.Canvas.Settings
{
    public sealed class CanvasSizeParameter(SizeSingle size) : IDefaultCheck
    {
        public SizeSingle Size { get; init; } = size;
        public bool IsDefault() => false;
    }
}
