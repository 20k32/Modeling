using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.PlatformHelpers.Windowing;
using System.Threading.Tasks;
using Windows.Storage;

namespace Modeling.PlatformHelpers.Providers.PackageRelatedProviders.ApplicationDataAccess
{
    sealed class PackagedApplicationDataProvider : IPackageRelatedApplicationDataProvider
    {
        readonly IWindowHelper _windowHelper;

        public StorageFolder LocalFolder => Windows.Storage.ApplicationData.Current.LocalFolder;

        public PackagedApplicationDataProvider()
        {
            _windowHelper = Ioc.Default.GetService<IWindowHelper>();
        }

        public async Task InitializeAsync(string _) => await _windowHelper.WindowInitializationTask;

        public object GetSettingsValue(string key) => Windows.Storage.ApplicationData.Current.LocalSettings.Values[key];

        public void SetSettingsValue(string key, object value) => Windows.Storage.ApplicationData.Current.LocalSettings.Values[key] = value;

        public void Remove(string key) => Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(key);

        public bool ContainsKey(string key) => Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey(key);
    }
}
