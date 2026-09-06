using Modeling.Core.Dispatching.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.Core.Dispatching
{
    public interface IUserInterfaceThreadContext
    {
        void Initialize(SynchronizationContext context);
        void StartExecuting<T>(Action<MainThreadMessage<MainThreadMessageState<T>>> action, MainThreadMessageState<T> state = default, CancellationToken token = default);
        Task ExecuteAsync<T>(Action<MainThreadMessage<MainThreadMessageState<T>>> action, MainThreadMessageState<T> state = default, CancellationToken token = default);
        Task ExecuteAsync<T>(Func<MainThreadMessage<MainThreadMessageState<T>>, Task> asyncAction, MainThreadMessageState<T> state = default, CancellationToken token = default);
        Task<T> ExecuteAsync<T>(Func<MainThreadMessage<MainThreadMessageState<T>>, Task<T>> asyncMethod, MainThreadMessageState<T> state = default, CancellationToken token = default);
    }
}
