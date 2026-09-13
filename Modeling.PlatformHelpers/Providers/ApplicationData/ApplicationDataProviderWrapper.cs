using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.PlatformHelpers.Providers.ApplicationPackage;
using Modeling.PlatformHelpers.Providers.PackageRelatedProviders.ApplicationDataAccess;
using System.Threading.Tasks;
using Windows.Storage;

namespace Modeling.PlatformHelpers.Providers.ApplicationData
{
    sealed class ApplicationDataProviderWrapper : IApplicationDataProvider
    {
        readonly IApplicationPackageChecker _applicationPackageChecker;

        IPackageRelatedApplicationDataProvider _applicationDataProvider;

        public StorageFolder LocalFolder => _applicationDataProvider.LocalFolder;

        public ApplicationDataProviderWrapper()
        {
            _applicationPackageChecker = Ioc.Default.GetRequiredService<IApplicationPackageChecker>();
        }

        public object GetSettingsValue(string key) => _applicationDataProvider.GetSettingsValue(key);

        public async Task InitializeAsync(string settingsFileName)
        {
            _applicationDataProvider = _applicationPackageChecker.CheckApplicationPackaged()
                ? Ioc.Default.GetRequiredService<PackagedApplicationDataProvider>()
                : Ioc.Default.GetRequiredService<UnpackagedApplicationDataProvider>();

            await _applicationDataProvider.InitializeAsync(settingsFileName);
        }
        public void SetSettingsValue(string key, object value) => _applicationDataProvider.SetSettingsValue(key, value);
    }
}
