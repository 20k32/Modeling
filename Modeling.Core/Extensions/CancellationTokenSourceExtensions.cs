using System;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.Core.Extensions
{
    public static class CancellationTokenSourceExtensions
    {
        public static async Task TryCancelAsync(this CancellationTokenSource tokenSource, bool shouldDispose = true)
        {
            if (tokenSource is not null)
            {
                try
                {
                    await tokenSource.CancelAsync();
                }
                catch (ObjectDisposedException)
                { }

                if (shouldDispose)
                {
                    tokenSource.Dispose();
                }
            }
        }
    }
}
