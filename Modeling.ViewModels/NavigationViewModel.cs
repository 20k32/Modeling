using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Modeling.Core.Abstractions;
using Modeling.Core.Navigation;

namespace Modeling.ViewModels
{
    public sealed partial class NavigationViewModel : ObservableObject
    {
        readonly INavigationProvider _navigationProvider;

        public NavigationViewModel()
        {
            _navigationProvider = Ioc.Default.GetService<INavigationProvider>();
        }


        [RelayCommand]
        void Initialize()
        {
            _navigationProvider.Initialize();
            _navigationProvider.Navigate(NavigationPage.SettingsPage);
        }
    }
}
