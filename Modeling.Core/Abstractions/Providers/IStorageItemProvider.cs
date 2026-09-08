using System.Threading.Tasks;

namespace Modeling.Core.Abstractions.Providers
{
    public interface IStorageItemProvider : IAsyncInitializer
    {
        Task<string> LoadContentAsync(string token);
        Task SaveContentAsync(string content, string token);
    }
}
