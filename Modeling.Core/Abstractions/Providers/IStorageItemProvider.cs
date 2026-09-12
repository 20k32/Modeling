using System.Threading.Tasks;

namespace Modeling.Core.Abstractions.Providers
{
    public interface IStorageItemProvider
    {
        string PathToFile { get; }
        Task<string> LoadContentAsync();
        Task SaveContentAsync(string content);
        Task InitializeAsync(string key, string token, string defaultFileNameWithExtension);
    }
}
