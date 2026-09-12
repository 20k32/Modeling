using System.Threading.Tasks;

namespace Modeling.Core.Abstractions.Providers
{
    public interface IApplicationSettingsProvider
    {
        Task<T> GetSettingsValueAsync<T>(string token);
        Task SetSettingsValueAsync<T>(string key, T value);
    }
}
