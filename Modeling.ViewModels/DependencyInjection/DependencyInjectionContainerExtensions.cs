using Microsoft.Extensions.DependencyInjection;
using Modeling.Models.DependencyInjection;
using Modeling.ViewModels.Pages.MainPage.Drawing;

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
