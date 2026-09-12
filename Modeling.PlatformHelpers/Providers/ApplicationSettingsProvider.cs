using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Providers;
using Modeling.PlatformHelpers.Windowing;
using System.Threading.Tasks;
using Windows.Storage;

namespace Modeling.PlatformHelpers.Providers
{
    sealed class ApplicationSettingsProvider : IApplicationSettingsProvider
    {
        readonly IWindowHelper _windowHelper;

        public ApplicationSettingsProvider()
        {
            _windowHelper = Ioc.Default.GetRequiredService<IWindowHelper>();
        }

        public async Task<T> GetSettingsValueAsync<T>(string token)
        {
            await _windowHelper.WindowInitializationTask;

            var result = default(T);

            var settingsValue = ApplicationData.Current.LocalSettings.Values[token];

            if (settingsValue is not null)
            {
                result = (T)settingsValue;
            }

            return result;
        }

        public async Task SetSettingsValueAsync<T>(string key, T value)
        {
            await _windowHelper.WindowInitializationTask;

            ApplicationData.Current.LocalSettings.Values[key] = value;
        }
    }
}
