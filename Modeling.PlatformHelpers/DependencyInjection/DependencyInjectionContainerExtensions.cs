using Microsoft.Extensions.DependencyInjection;
using Modeling.Core.Abstractions.Providers;
using Modeling.PlatformHelpers.Monitor;
using Modeling.PlatformHelpers.Providers;
using Modeling.PlatformHelpers.Providers.ApplicationData;
using Modeling.PlatformHelpers.Providers.ApplicationPackage;
using Modeling.PlatformHelpers.Providers.FutureAccessList;
using Modeling.PlatformHelpers.Providers.PackageRelatedProviders.ApplicationDataAccess;
using Modeling.PlatformHelpers.Providers.PackageRelatedProviders.FutureAccessList;
using Modeling.PlatformHelpers.Screens;
using Modeling.PlatformHelpers.Windowing;

namespace Modeling.PlatformHelpers.DependencyInjection
{
    public static class DependencyInjectionContainerExtensions
    {
        public static IServiceCollection RegisterPlatformHelpers(this IServiceCollection services)
            => services.AddSingleton<IApplicationPackageChecker, ApplicationPackageChecker>()
                .AddTransient<UnpackagedApplicationDataProvider>()
                .AddSingleton<PackagedApplicationDataProvider>()
                .AddSingleton<IApplicationDataProvider, ApplicationDataProviderWrapper>()
                .AddSingleton<PackagedFutureAccessListProvider>()
                .AddSingleton<UnpackagedFutureAccessListProvider>()
                .AddSingleton<IFutureAccessListProvider, FutureAccessListProviderWrapper>()
                .AddTransient<IStorageItemProvider, StorageItemProvider>()
                .AddSingleton<IApplicationKeyProvider, ApplicationKeyProvider>()
                .AddSingleton<IApplicationSettingsProvider, ApplicationSettingsProvider>()
                .AddSingleton<IScreenListener, ScreenListener>()
                .AddSingleton<IWindowHelper, WindowHelper>();
    }
}
