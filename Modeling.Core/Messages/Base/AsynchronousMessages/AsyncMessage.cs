using CommunityToolkit.Mvvm.Messaging.Messages;
using Modeling.Core.Abstractions;
using System.Threading.Tasks;

namespace Modeling.Core.Messages.Base.AsynchronousMessages
{
    public class AsyncMessage(object sender) : AsyncRequestMessage<Task>, IDefaultCheck
    {
        public object Sender { get; init; } = sender;

        public bool IsDefault() => Sender is null;
    }
}
