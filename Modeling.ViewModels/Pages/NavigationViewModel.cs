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
            _navigationProvider = Ioc.Default.GetRequiredService<INavigationProvider>();

            var lab1NavigationItem = new NavigationItem("Lab 1", NavigationPage.FirstLabPage);

            lab1NavigationItem.Children.AddRange([
                new NavigationItem("Resizing", NavigationPage.ResizingPage),
                new NavigationItem("Affine", NavigationPage.AffineTransformPage),
                new NavigationItem("Projective", NavigationPage.ProjectiveTransformPage)]);

            var lab2NavigationItem = new NavigationItem("Lab 2", NavigationPage.SecondLabPage);

            _navigationItems = [
                    lab1NavigationItem,
                    lab2NavigationItem,
                    new NavigationItem("Settings", NavigationPage.SettingsPage)];
        }

        async partial void OnSelectedNavigationItemChanged(NavigationItem newValue)
        {
            if (newValue is null || _navigationProvider.CurrentPage == newValue.NavigationPage)
            {
                return;
            }

            await _navigationProvider.NavigateAsync(newValue.NavigationPage);
        }

        [RelayCommand]
        void Initialize()
        {
            _navigationProvider.Initialize();

            SelectedNavigationItem = NavigationItems.First(item => item.NavigationPage == NavigationPage.SettingsPage);
        }
    }
}
