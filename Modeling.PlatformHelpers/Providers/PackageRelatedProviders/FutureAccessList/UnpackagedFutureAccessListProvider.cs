using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.PlatformHelpers.Providers.PackageRelatedProviders.ApplicationDataAccess;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Modeling.PlatformHelpers.Providers.PackageRelatedProviders.FutureAccessList
{
    sealed class UnpackagedFutureAccessListProvider : IPackageRelatedFutureAccessListProvider
    {
        readonly IPackageRelatedApplicationDataProvider _unpackagedApplicationDataProvider;

        public UnpackagedFutureAccessListProvider()
        {
            _unpackagedApplicationDataProvider = Ioc.Default.GetRequiredService<UnpackagedApplicationDataProvider>();
        }

        public string Add(IStorageItem item)
        {
            _unpackagedApplicationDataProvider.SetSettingsValue(item.Path, default);
            return item.Path;
        }

        public bool ContainsItem(string token) => _unpackagedApplicationDataProvider.ContainsKey(token);

        public async Task<StorageFile> GetFileAsync(string token)
            => await StorageFile.GetFileFromPathAsync(token);

        public async Task InitializeAsync(string defaultFileNameWithExtension)
        {
            await _unpackagedApplicationDataProvider.InitializeAsync(defaultFileNameWithExtension);
        }

        public void Remove(string token) => _unpackagedApplicationDataProvider.Remove(token);
    }
}
