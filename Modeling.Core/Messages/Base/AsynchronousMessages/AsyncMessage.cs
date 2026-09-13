using CommunityToolkit.Mvvm.Messaging.Messages;
using Modeling.Core.Abstractions;
using Modeling.Core.Miscellaneous;

namespace Modeling.Core.Messages.Base.AsynchronousMessages
{
    public class AsyncMessage(object sender) : AsyncRequestMessage<Unit>, IDefaultCheck
    {
        public object Sender { get; init; } = sender;

        public bool IsDefault() => Sender is null;
    }
}
