using Modeling.Core.Messages.Base.SynchronousMessages;

namespace Modeling.Core.Messages.Canvas.Settings
{
    public sealed class InitializeDrawingSessionMessage : Message
    {
        public InitializeDrawingSessionMessage(object sender) : base(sender)
        { }
    }
}
