using System.Threading.Tasks;

namespace Modeling.Core.Abstractions.Providers
{
    public interface IApplicationSettingsProvider
    {
        T GetSettingsValue<T>(string token);
        void SetSettingsValue<T>(string key, T value);
        Task InitializeAsync(string defaultFileNameWithExtension);
    }
}
