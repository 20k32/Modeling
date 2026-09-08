using System.Threading.Tasks;

namespace Modeling.Core.Abstractions.Providers
{
    internal interface IApplicationSettingsProvider
    {
        Task<T> GetSettingsValueAsync<T>(string token);
    }
}
