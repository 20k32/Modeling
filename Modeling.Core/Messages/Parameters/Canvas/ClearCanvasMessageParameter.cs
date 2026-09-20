using Modeling.Core.Abstractions;
using Modeling.Core.Drawing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modeling.Core.Messages.Parameters.Canvas
{
    public sealed class ClearCanvasMessageParameter(DrawingColor color, bool clearBeforeRedraw, IObjectTree parent = default) : IDefaultCheck, IObjectTree
    {
        public IObjectTree Parent { get; init; } = parent;
        public DrawingColor Color { get; init; } = color;
        public bool ClearBeforeRedraw { get; init; } = clearBeforeRedraw;

        public bool IsDefault() => Color?.IsDefault() ?? true;
    }
}
