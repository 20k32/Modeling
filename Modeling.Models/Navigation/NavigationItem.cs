using CommunityToolkit.Mvvm.ComponentModel;
using Modeling.Core.Navigation;

namespace Modeling.Models.Navigation
{
    public sealed partial class NavigationItem(string name, NavigationPage page) : ObservableObject
    {
        [ObservableProperty]
        string _itemName = name;

        public readonly NavigationPage NavigationPage = page;
    }
}
