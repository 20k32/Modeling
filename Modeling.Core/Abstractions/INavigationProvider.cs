using Modeling.Core.CoreDelegates;
using Modeling.Core.Navigation;
using Modeling.Models.Navigation;
using System;
using System.Threading.Tasks;

namespace Modeling.Core.Abstractions
{
    public interface INavigationProvider
    {
        event AsyncActionEventHandler<NavigatingCancellationParameter> Navigating;
        event AsyncActionEventHandler<NavigationParameter> Navigated;

        NavigationPage CurrentPage { get; }

        void AddNavigationPageMapping(Type type, NavigationPage page);

        void Initialize();

        Task NavigateAsync(NavigationPage page);
        Task NavigateBackAsync();
    }
}
