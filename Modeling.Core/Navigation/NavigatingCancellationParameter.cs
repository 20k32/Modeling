namespace Modeling.Core.Navigation
{
    public sealed class NavigatingCancellationParameter(NavigationPage fromPage, NavigationPage toPage, bool cancelNavigation)
    {
        public bool CancelNavigation { get; set; } = cancelNavigation;
        public NavigationPage FromPage { get; } = fromPage;
        public NavigationPage ToPage { get; } = toPage;
    }
}
