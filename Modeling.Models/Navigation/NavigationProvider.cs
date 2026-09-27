using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;
using Modeling.Core.Abstractions;
using Modeling.Core.CoreDelegates;
using Modeling.Core.Navigation;
using Modeling.Models.UserInterface;
using Modeling.PlatformHelpers.Windowing;
using System;
using System.Collections.Concurrent;

namespace Modeling.Models.Navigation
{
    sealed class NavigationProvider : INavigationProvider
    {
        private readonly ConcurrentDictionary<NavigationPage, Control> _navigationPageMappings = new ConcurrentDictionary<NavigationPage, Control>();
        private readonly ConcurrentDictionary<NavigationPage, Type> _navigationPageSources = new ConcurrentDictionary<NavigationPage, Type>();

        readonly IWindowHelper _windowHelper;
        readonly IUserInterfaceConstantsProvider _userInterfaceConstants;

        ContentControl _navigationControl;

        NavigationPage _previousPage;
        NavigationPage _currentPage;

        public NavigationPage CurrentPage => _currentPage;

        public event ActionEventHandler<NavigatingCancellationParameter> Navigating;
        public event ActionEventHandler<NavigationPage> Navigated;

        public NavigationProvider()
        {
            _windowHelper = Ioc.Default.GetService<IWindowHelper>();
            _userInterfaceConstants = Ioc.Default.GetService<IUserInterfaceConstantsProvider>();
        }

        public void Initialize()
        {
            _navigationControl = _windowHelper.MainWindow.Content
                .FindDescendant<ContentControl>(contentControl => contentControl.Tag as string == _userInterfaceConstants.NavigationFrameName);
        }

        public void AddNavigationPageMapping(Type pageControl, NavigationPage page)
        {
            _navigationPageSources.TryAdd(page, pageControl);
        }

        public void Navigate(NavigationPage page)
        {
            var cancelNavigationParameter = new NavigatingCancellationParameter(
                fromPage: _currentPage,
                toPage: page,
                cancelNavigation: false);

            Navigating?.Invoke(cancelNavigationParameter);

            if (!cancelNavigationParameter.CancelNavigation)
            {
                if (_navigationPageMappings.TryGetValue(page, out var pageToNavigate) && !(pageToNavigate is null))
                {
                    _navigationControl.Content = pageToNavigate;
                }
                else if (_navigationPageSources.TryGetValue(page, out var pageTypeToNavigate) && !(pageTypeToNavigate is null))
                {
                    var pageInstance = Activator.CreateInstance(pageTypeToNavigate) as Control;

                    if (!(pageInstance is null) && _navigationPageMappings.TryAdd(page, pageInstance))
                    {
                        _navigationControl.Content = pageInstance;
                    }
                }

                Navigated?.Invoke(page);

                _previousPage = _currentPage;
                _currentPage = page;
            }
        }

        public void NavigateBack() => Navigate(_previousPage);
    }
}
