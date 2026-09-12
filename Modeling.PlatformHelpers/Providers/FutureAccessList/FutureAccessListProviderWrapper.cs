using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.PlatformHelpers.Providers.ApplicationPackage;
using Modeling.PlatformHelpers.Providers.PackageRelatedProviders.FutureAccessList;
using System.Threading.Tasks;
using Windows.Storage;

namespace Modeling.PlatformHelpers.Providers.FutureAccessList
{
    sealed class FutureAccessListProviderWrapper : IFutureAccessListProvider
    {
        readonly IApplicationPackageChecker _applicationPackageChecker;

        public FutureAccessListProviderWrapper()
        {
            _applicationPackageChecker = Ioc.Default.GetRequiredService<IApplicationPackageChecker>();
        }

        IPackageRelatedFutureAccessListProvider _packageRelatedFutureAccessListProvider;

        public string Add(IStorageItem item) => _packageRelatedFutureAccessListProvider.Add(item);

        public bool ContainsItem(string token)
            => !string.IsNullOrWhiteSpace(token) && _packageRelatedFutureAccessListProvider.ContainsItem(token);

        public async Task<StorageFile> GetFileAsync(string token) =>
           await _packageRelatedFutureAccessListProvider.GetFileAsync(token);

        public async Task InitializeAsync(string defaultFileNameWithExtension)
        {
            _packageRelatedFutureAccessListProvider = _applicationPackageChecker.CheckApplicationPackaged()
                ? Ioc.Default.GetRequiredService<PackagedFutureAccessListProvider>()
                : Ioc.Default.GetRequiredService<UnpackagedFutureAccessListProvider>();

            await _packageRelatedFutureAccessListProvider.InitializeAsync(defaultFileNameWithExtension);
        }

        public void Remove(string token) => _packageRelatedFutureAccessListProvider.Remove(token);
    }
}
