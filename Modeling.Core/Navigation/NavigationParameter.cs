using Modeling.Core.Navigation;

namespace Modeling.Models.Navigation
{
    public class NavigationParameter(NavigationPage fromPage, NavigationPage toPage)
    {
        public NavigationPage FromPage { get; } = fromPage;
        public NavigationPage ToPage { get; } = toPage;
    }
}
