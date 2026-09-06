using Microsoft.Extensions.DependencyInjection;
using Modeling.Core.DependencyInjection;
using Modeling.PlatformHelpers.DependencyInjection;

namespace Modeling.Models.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterModelsServices(this IServiceCollection services)
            => services.RegisterCoreServices()
            .RegisterPlatformHelpers();
    }
}
