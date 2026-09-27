using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Modeling.Core.Abstractions;
using Modeling.Core.Dispatching;
using Modeling.Core.Logging;
using Modeling.Core.Navigation;
using Modeling.PlatformHelpers.Windowing;
using Modeling.UI.DependencyInjection;
using Modeling.UI.Resources.Controls.Pages;
using System.Threading;

namespace Modeling.UI
{
    public partial class App : Application
    {
        static App()
        {
            Ioc.Default.ConfigureContainer();

            Ioc.Default.GetRequiredService<IUserInterfaceThreadContext>().Initialize(SynchronizationContext.Current);
        }

        public App()
        {
            InitializeComponent();

            InitializeNavigation();

            Logger.Information("Application initialized");
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var windowHelper = Ioc.Default.GetRequiredService<IWindowHelper>();

            windowHelper.MainWindow = new MainWindow();

            windowHelper.CenterMainWindow();
            windowHelper.ActivateApplicationWindow();

            Logger.Information("Activated application window");
        }

        static void InitializeNavigation()
        {
            var navigationProvider = Ioc.Default.GetRequiredService<INavigationProvider>();

            navigationProvider.AddNavigationPageMapping(typeof(ResizingPage), NavigationPage.ResizingPage);
            navigationProvider.AddNavigationPageMapping(typeof(AffineTransformPage), NavigationPage.AffineTransformPage);
            navigationProvider.AddNavigationPageMapping(typeof(EuclideanTransformPage), NavigationPage.EuclideanTransformPage);
            navigationProvider.AddNavigationPageMapping(typeof(ProjectiveTransformPage), NavigationPage.ProjectiveTransformPage);
            navigationProvider.AddNavigationPageMapping(typeof(SettingsPage), NavigationPage.SettingsPage);
        }
    }
}
