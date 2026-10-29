using Modeling.Models.Navigation;

namespace Modeling.Core.Navigation
{
    public sealed class NavigatingCancellationParameter(NavigationPage fromPage, NavigationPage toPage, bool cancelNavigation) 
        : NavigationParameter(fromPage, toPage)
    {
        public bool CancelNavigation { get; set; } = cancelNavigation;
    }
}
