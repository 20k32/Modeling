using Modeling.Core.Abstractions;

namespace Modeling.Core.Messages.Base.SynchronousMessages
{
    public class Message : IDefaultCheck
    {
        public object Sender { get; init; }
        public Message(object sender) => Sender = sender;

        public virtual bool IsDefault() => Sender is null;
    }
}
