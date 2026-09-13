using System.Threading.Tasks;
using Windows.Storage;

namespace Modeling.PlatformHelpers.Providers.FutureAccessList
{
    public interface IFutureAccessListProvider
    {
        Task InitializeAsync(string defaultFileNameWithExtension);

        bool ContainsItem(string token);
        string Add(IStorageItem item);
        void Remove(string token);
        Task<StorageFile> GetFileAsync(string token);
    }
}
