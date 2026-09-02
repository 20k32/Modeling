using Microsoft.Extensions.DependencyInjection;
using Modeling.PlatformHelpers.Windowing;

namespace Modeling.PlatformHelpers.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterPlatformHelpers(this ServiceCollection services)
            => services.AddSingleton<IWindowHelper, WindowHelper>();
    }
}
