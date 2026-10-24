using Modeling.Core.Navigation;

namespace Modeling.Models.Navigation
{
    public sealed class NavigatedArguments(NavigationPage fromPage, NavigationPage toPage)
    {
        public readonly NavigationPage FromPage = fromPage;
        public readonly NavigationPage ToPage = toPage;
    }
}
