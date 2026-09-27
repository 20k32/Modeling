using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Modeling.Core.Abstractions;
using Modeling.Core.Navigation;
using Modeling.Models.Navigation;
using System.Collections.ObjectModel;
using System.Linq;

namespace Modeling.ViewModels
{
    public sealed partial class NavigationViewModel : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<NavigationItem> _navigationItems;

        [ObservableProperty]
        NavigationItem _selectedNavigationItem;

        readonly INavigationProvider _navigationProvider;

        public NavigationViewModel()
        {
            _navigationProvider = Ioc.Default.GetService<INavigationProvider>();

            _navigationItems = [
                new NavigationItem("Resizing", NavigationPage.ResizingPage),
                new NavigationItem("Affine", NavigationPage.AffineTransformPage),
                new NavigationItem("Projective", NavigationPage.ProjectiveTransformPage),
                new NavigationItem("Settings", NavigationPage.SettingsPage)];
        }

        partial void OnSelectedNavigationItemChanged(NavigationItem newValue)
        {
            if (newValue is null || _navigationProvider.CurrentPage == newValue.NavigationPage)
            {
                return;
            }

            _navigationProvider.Navigate(newValue.NavigationPage);
        }

        [RelayCommand]
        void Initialize()
        {
            _navigationProvider.Initialize();

            SelectedNavigationItem = NavigationItems.First(item => item.NavigationPage == NavigationPage.SettingsPage);
        }
    }
}
