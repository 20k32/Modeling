using CommunityToolkit.Mvvm.ComponentModel;
using Modeling.Core.Constants;
using Modeling.Core.CoreDelegates;
using Modeling.Core.Extensions;
using Modeling.Core.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.ViewModels.Pages
{
    public abstract class BaseViewModel : ObservableObject
    {
        readonly SemaphoreSlim _applyingChangesLock;

        CancellationTokenSource _applyingChangesCancellation;

        protected BaseViewModel()
        {
            _applyingChangesLock = new SemaphoreSlim(1, 1);
        }

        protected async Task WaitBeforeExecutionAsync(ActionEventHandler action)
        {
            try
            {
                await _applyingChangesCancellation.TryCancelAsync(shouldDispose: false);

                using (var cancellationSource = new CancellationTokenSource())
                {
                    _applyingChangesCancellation = cancellationSource;

                    await Task.Delay(CoreConstants.MAXIMUM_DELAY_BEFORE_CHANGES_APPLIED_MILLISECONDS, cancellationSource.Token);

                    await _applyingChangesLock.WaitAsync();

                    action();
                }
            }
            catch (Exception ex)
            {
                if (ex is not OperationCanceledException)
                {
                    Logger.Exception(ex);
                }
            }
            finally
            {
                _applyingChangesLock.ReleaseSafe();
            }
        }

        protected async Task WaitBeforeExecutionAsync(AsyncActionEventHandler asyncAction)
        {
            try
            {
                await _applyingChangesCancellation.TryCancelAsync(shouldDispose: false);

                using (var cancellationSource = new CancellationTokenSource())
                {
                    _applyingChangesCancellation = cancellationSource;

                    await Task.Delay(CoreConstants.MAXIMUM_DELAY_BEFORE_CHANGES_APPLIED_MILLISECONDS, cancellationSource.Token);

                    await _applyingChangesLock.WaitAsync();

                    await asyncAction();
                }
            }
            catch (Exception ex)
            {
                if (ex is not OperationCanceledException)
                {
                    Logger.Exception(ex);
                }
            }
            finally
            {
                _applyingChangesLock.ReleaseSafe();
            }
        }

        protected async Task WaitBeforeExecutionAsync<T>(AsyncActionEventHandler<T> asyncAction, T value)
        {
            try
            {
                await _applyingChangesCancellation.TryCancelAsync(shouldDispose: false);

                using (var cancellationSource = new CancellationTokenSource())
                {
                    _applyingChangesCancellation = cancellationSource;

                    await Task.Delay(CoreConstants.MAXIMUM_DELAY_BEFORE_CHANGES_APPLIED_MILLISECONDS, cancellationSource.Token);

                    await _applyingChangesLock.WaitAsync();

                    await asyncAction(value);
                }
            }
            catch (Exception ex)
            {
                if (ex is not OperationCanceledException)
                {
                    Logger.Exception(ex);
                }
            }
            finally
            {
                _applyingChangesLock.ReleaseSafe();
            }
        }
    }
}
