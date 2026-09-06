using System.Threading;

namespace Modeling.Core.Dispatching.Messages
{
    public sealed class MainThreadMessage<T>
    {
        public readonly T State;
        public readonly CancellationToken CancellationToken;

        public MainThreadMessage(T state = default, CancellationToken cancellationToken = default)
        {
            State = state;
            CancellationToken = cancellationToken;
        }
    }
}
