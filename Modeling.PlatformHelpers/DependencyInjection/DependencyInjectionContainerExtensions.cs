using Microsoft.Extensions.DependencyInjection;
using Modeling.Core.Abstractions.Providers;
using Modeling.PlatformHelpers.Monitor;
using Modeling.PlatformHelpers.Providers;
using Modeling.PlatformHelpers.Screens;
using Modeling.PlatformHelpers.Windowing;

namespace Modeling.PlatformHelpers.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterPlatformHelpers(this IServiceCollection services)
            => services.AddSingleton<IApplicationKeyProvider, ApplicationKeyProvider>()
                .AddSingleton<IApplicationSettingsProvider, ApplicationSettingsProvider>()
                .AddSingleton<IScreenListener, ScreenListener>()
                .AddSingleton<IWindowHelper, WindowHelper>();
    }
}
