using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Modeling.Models.Abstractions.Dialogs;
using Modeling.UI.Resources.Dialogs;
using Modeling.ViewModels.DependencyInjection;

namespace Modeling.UI.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static void ConfigureContainer(this Ioc container)
            => container.ConfigureServices(
                new ServiceCollection()
                .RegisterViewModelServices()
                .AddTransient<ISettingsDialog, SettingsDialog>()
                .BuildServiceProvider());
    }
}
