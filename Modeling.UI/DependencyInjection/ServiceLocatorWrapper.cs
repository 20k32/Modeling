using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.ViewModels;

namespace Modeling.UI.DependencyInjection
{
    sealed class ServiceLocatorWrapper
    {
        public ServiceLocatorWrapper()
        { }

        public static DrawingViewModel DrawingViewModel => Ioc.Default.GetRequiredService<DrawingViewModel>();
        public static SettingsViewModel SettingsViewModel => Ioc.Default.GetRequiredService<SettingsViewModel>();
    }
}
