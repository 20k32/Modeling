using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Modeling.Core.Dispatching;
using Modeling.Core.Logging;
using Modeling.PlatformHelpers.Windowing;
using Modeling.UI.DependencyInjection;
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
    }
}
