using Microsoft.Extensions.DependencyInjection;
using Modeling.PlatformHelpers.DependencyInjection;

namespace Modeling.Models.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterModelsServices(this ServiceCollection services)
            => services.RegisterPlatformHelpers();
    }
}
