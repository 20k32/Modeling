using Modeling.Core.Abstractions;
using Modeling.Models.Drawing.DrawingMessageValues.Points;
using Modeling.Models.Enums;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Modeling.Models.Drawing.DrawingMessageValues
{
    public abstract class DrawingMessageValue : IDefaultCheck
    {
        public DrawingMessageType MessageType { get; protected set; }
        public IEnumerable<DrawMessageValue> DrawingParameters => pointMessages;

        protected readonly Queue<DrawMessageValue> pointMessages;

        protected DrawingMessageValue(DrawingMessageType messageType)
        {
            MessageType = messageType;
            pointMessages = [];
        }

        protected bool ValidateDrawingParameter(DrawMessageValue value)
            => value is not null && !value.IsDefault();

        public abstract void AddDrawingParameter(DrawMessageValue value);

        public virtual bool IsDefault() => MessageType == DrawingMessageType.NoAction || DrawingParameters is null;
    }
}
