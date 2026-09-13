using Modeling.Core.Abstractions;
using Modeling.Core.Drawing;
using Modeling.Models.Enums;

namespace Modeling.Models.Drawing.DrawingMessageValues
{
    public abstract class DrawingMessageValue : IDefaultCheck
    {
        public DrawingMessageType MessageType { get; protected set; }
        public DrawingColor Color { get; protected set; }

        protected DrawingMessageValue(DrawingColor color) => Color = color;

        public virtual bool IsDefault() => Color is null;
    }
}
