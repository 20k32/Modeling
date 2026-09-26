using Modeling.Core.Abstractions;

namespace Modeling.Core.Messages.Parameters.Canvas.Drawing
{
    public abstract class DrawingMessageParameter(bool clearBeforeRedraw, IObjectTree parent) : IDefaultCheck, IObjectTree
    {
        public IObjectTree Parent { get; init; } = parent;

        public bool ClearBeforeRedraw { get; init; } = clearBeforeRedraw;

        public abstract bool IsDefault();
    }
}
