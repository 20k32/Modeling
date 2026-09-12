using Modeling.Core.Dispatching.Messages;
using Modeling.Core.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.Core.Dispatching
{
    public sealed class UserInterfaceThreadContext : IUserInterfaceThreadContext
    {
        SynchronizationContext _context;

        MainThreadMessage<MainThreadMessageState<T>> ConfigureMainThreadMessage<T>(MainThreadMessageState<T> state = default, CancellationToken token = default)
        {
            return new MainThreadMessage<MainThreadMessageState<T>>(state ?? MainThreadMessageState<T>.Default, cancellationToken: token);
        }

        public void StartExecuting<T>(Action<MainThreadMessage<MainThreadMessageState<T>>> action, MainThreadMessageState<T> state = default, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            var message = ConfigureMainThreadMessage(state, token);

            _context.Post((internalState) =>
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    action(internalState as MainThreadMessage<MainThreadMessageState<T>>);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }, message);
        }

        public async Task ExecuteAsync<T>(Action<MainThreadMessage<MainThreadMessageState<T>>> action, MainThreadMessageState<T> state = default, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            var message = ConfigureMainThreadMessage(state, token);

            var mainThreadLogicCompletionSource = new TaskCompletionSource();

            _context.Post(async (internalState) =>
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    action(internalState as MainThreadMessage<MainThreadMessageState<T>>);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }, message);

            await mainThreadLogicCompletionSource.Task;
        }

        public async Task ExecuteAsync<T>(Func<MainThreadMessage<MainThreadMessageState<T>>, Task> asyncAction, MainThreadMessageState<T> state = default, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            var message = ConfigureMainThreadMessage(state, token);

            var mainThreadLogicCompletionSource = new TaskCompletionSource();

            _context.Post(async (internalState) =>
            {
                try
                {
                    token.ThrowIfCancellationRequested();

                    await asyncAction(internalState as MainThreadMessage<MainThreadMessageState<T>>);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
            }, message);

            await mainThreadLogicCompletionSource.Task;
        }

        public async Task<T> ExecuteAsync<T>(Func<MainThreadMessage<MainThreadMessageState<T>>, Task<T>> asyncMethod, MainThreadMessageState<T> state = default, CancellationToken token = default)
        {
            if (token.IsCancellationRequested)
            {
                return default;
            }

            var message = ConfigureMainThreadMessage(state, token);

            var mainThreadLogicCompletionSource = new TaskCompletionSource<T>();

            _context.Post(async (internalState) =>
            {
                T result = default;

                try
                {
                    token.ThrowIfCancellationRequested();

                    result = await asyncMethod(internalState as MainThreadMessage<MainThreadMessageState<T>>);
                }
                catch (Exception ex)
                {
                    Logger.Exception(ex);
                }
                finally
                {
                    mainThreadLogicCompletionSource.TrySetResult(result);
                }
            }, message);

            return await mainThreadLogicCompletionSource.Task;
        }

        public void Initialize(SynchronizationContext context) => _context = context;
    }
}
