using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Modeling.Core.Dispatching;
using Modeling.Core.Logging;
using Modeling.Core.Miscellaneous;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.ViewModels
{
    public sealed partial class DrawingViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task InitializeAsync()
        {
            await Ioc.Default.GetService<IUserInterfaceThreadContext>().ExecuteAsync<Unit>((_) =>
            {
                var id = Thread.CurrentThread.ManagedThreadId;
                Logger.Information("Content loaded");
            });

        }
    }
}
