using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Providers;
using Modeling.PlatformHelpers.Windowing;
using System.Threading.Tasks;

namespace Modeling.PlatformHelpers.Providers
{
    sealed class ApplicationKeyProvider : IApplicationKeyProvider
    {
        const string APPLICATION_KEY = "ModelingAppKey";

        readonly IWindowHelper _windowHelper;

        public string DrawingSettingsTokenKey => APPLICATION_KEY;

        public ApplicationKeyProvider()
        {
            _windowHelper = Ioc.Default.GetService<IWindowHelper>();
        }

        public async Task InitializeAsync() => await _windowHelper.WindowInitializationTask;
    }
}
