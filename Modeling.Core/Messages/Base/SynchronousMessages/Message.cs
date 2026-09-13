using Modeling.Core.Abstractions;

namespace Modeling.Core.Messages.Base.SynchronousMessages
{
    public class Message(object sender) : IDefaultCheck
    {
        public object Sender { get; init; } = sender;

        public virtual bool IsDefault() => Sender is null;
    }
}
