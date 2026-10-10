using Modeling.Core.Abstractions;
using Modeling.Core.Enums;

namespace Modeling.Core.Messages.Parameters.Canvas.Drawing
{
    public abstract class DrawingMessageParameter(bool clearBeforeRedraw, IObjectTree parent, bool applyAliasing, DrawingAliasing drawingAliasing) : IDefaultCheck, IObjectTree
    {
        public IObjectTree Parent { get; init; } = parent;

        public bool ApplyAliasing { get; init; } = applyAliasing;
        public DrawingAliasing Aliasing { get; init; } = drawingAliasing;

        public bool ClearBeforeRedraw { get; init; } = clearBeforeRedraw;

        public abstract bool IsDefault();
    }
}
