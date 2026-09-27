using Modeling.Core.CoreDelegates;
using Modeling.Core.Navigation;
using System;

namespace Modeling.Core.Abstractions
{
    public interface INavigationProvider
    {
        event ActionEventHandler<NavigatingCancellationParameter> Navigating;
        event ActionEventHandler Navigated;

        NavigationPage CurrentPage { get; }

        void AddNavigationPageMapping(Type type, NavigationPage page);

        void Initialize();

        void Navigate(NavigationPage page);
        void NavigateBack();
    }
}
