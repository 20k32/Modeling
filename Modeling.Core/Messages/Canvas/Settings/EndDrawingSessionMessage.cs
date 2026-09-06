using Modeling.Core.Messages.Base.SynchronousMessages;

namespace Modeling.Core.Messages.Canvas.Settings
{
    public sealed class EndDrawingSessionMessage : Message
    {
        public EndDrawingSessionMessage(object sender) : base(sender)
        { }
    }
}
