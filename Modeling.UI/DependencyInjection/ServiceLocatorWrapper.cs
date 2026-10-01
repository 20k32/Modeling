using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Models.UserInterface;
using Modeling.ViewModels;
using Modeling.ViewModels.Pages.MainPage.Drawing;

namespace Modeling.UI.DependencyInjection
{
    sealed class ServiceLocatorWrapper
    {
        public ServiceLocatorWrapper()
        { }

        public static NavigationViewModel NavigationViewModel => Ioc.Default.GetRequiredService<NavigationViewModel>();
        public static DrawingViewModel DrawingViewModel => Ioc.Default.GetRequiredService<DrawingViewModel>();
        public static SettingsViewModel SettingsViewModel => Ioc.Default.GetRequiredService<SettingsViewModel>();
        public static IUserInterfaceConstantsProvider UserInterfaceConstants => Ioc.Default.GetRequiredService<IUserInterfaceConstantsProvider>();
    }
}
