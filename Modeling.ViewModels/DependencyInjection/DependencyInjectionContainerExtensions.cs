using Microsoft.Extensions.DependencyInjection;
using Modeling.Models.DependencyInjection;
using Modeling.ViewModels.Pages.MainPage.Drawing;
using Modeling.ViewModels.Pages.MainPage.Settings;

namespace Modeling.ViewModels.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterViewModelServices(this IServiceCollection services)
            => services.RegisterModelsServices()
            .AddSingleton<NavigationViewModel>()
            .AddSingleton<SettingsViewModel>()
            .AddSingleton<DrawingViewModel>();
    }
}
