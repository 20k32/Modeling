using Microsoft.Extensions.DependencyInjection;
using Modeling.Models.DependencyInjection;

namespace Modeling.ViewModels.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterViewModelServices(this IServiceCollection services)
            => services.RegisterModelsServices()
            .AddSingleton<SettingsViewModel>()
            .AddSingleton<DrawingViewModel>();
    }
}
