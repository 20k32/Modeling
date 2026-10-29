using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Navigation;
using Modeling.Models.Abstractions.Collections.General;

namespace Modeling.Models.Navigation
{
    public sealed partial class NavigationItem(string name, NavigationPage page) : ObservableObject
    {
        [ObservableProperty]
        string _itemName = name;

        [ObservableProperty]
        INavigationItemsObservableCollection children = Ioc.Default.GetRequiredService<INavigationItemsObservableCollection>();

        public readonly NavigationPage NavigationPage = page;
    }
}
