using System;
using System.Threading;

namespace Modeling.Core.Extensions
{
    public static class SemaphoreExtensions
    {
        const int DEFAULT_SEMAPHORE_MAX_COUNT = 1;
        public static void ReleaseSafe(this SemaphoreSlim semaphore, int semaphoreMaxCount = DEFAULT_SEMAPHORE_MAX_COUNT)
        {
            if ((semaphore?.CurrentCount ?? semaphoreMaxCount) < semaphoreMaxCount)
            {
                try
                {
                    semaphore.Release();
                }
                catch (Exception ex) when (ex is ObjectDisposedException)
                { }
            }
        }
    }
}
