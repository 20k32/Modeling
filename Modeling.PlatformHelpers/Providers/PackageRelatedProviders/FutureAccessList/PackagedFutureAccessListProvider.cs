using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.PlatformHelpers.Windowing;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Modeling.PlatformHelpers.Providers.PackageRelatedProviders.FutureAccessList
{
    sealed class PackagedFutureAccessListProvider : IPackageRelatedFutureAccessListProvider
    {
        readonly IWindowHelper _windowHelper;

        public PackagedFutureAccessListProvider()
        {
            _windowHelper = Ioc.Default.GetRequiredService<IWindowHelper>();
        }

        public async Task InitializeAsync(string _)
            => await _windowHelper.WindowInitializationTask;

        public string Add(IStorageItem item)
            => StorageApplicationPermissions.FutureAccessList.Add(item);

        public bool ContainsItem(string token)
            => StorageApplicationPermissions.FutureAccessList.ContainsItem(token);

        public void Remove(string token)
            => StorageApplicationPermissions.FutureAccessList.Remove(token);

        public async Task<StorageFile> GetFileAsync(string token) =>
            await StorageApplicationPermissions.FutureAccessList
            .GetFileAsync(token)
            .AsTask();
    }
}
