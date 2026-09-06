using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.UI.Xaml;
using Modeling.Core.Constants;
using Modeling.PlatformHelpers.Miscellaneous;
using Modeling.PlatformHelpers.Screens;
using System;
using System.Linq;
using Windows.Graphics;

namespace Modeling.PlatformHelpers.Windowing
{
    sealed class WindowHelper : IWindowHelper
    {
        private Window _mainWindow;
        public Window MainWindow
        {
            get => _mainWindow;
            set
            {
                if (_mainWindow is not null)
                {
                    throw new ArgumentOutOfRangeException(string.Format(Constants.ArgumentNotNullExceptionFormat, _mainWindow));
                }
                else if (value is null)
                {
                    throw new ArgumentNullException(string.Format(Constants.ArgumentNullExceptionFormat, _mainWindow));
                }
                else
                {
                    _mainWindow = value;
                }
            }
        }

        private bool CanChangeMainWindow => _mainWindow is not null;

        public void ActivateApplicationWindow()
        {
            MainWindow?.Activate();
        }

        public void CenterMainWindow()
        {
            if (CanChangeMainWindow)
            {
                var primaryScreenLocation = Ioc.Default.GetService<IScreenListener>()
                    .Locations.First(location => location.IsPrimary);

                var centerX = (int)(primaryScreenLocation.ScaledSize.Width - ApplicationWindowConstants.DesignWidth) / 2;
                var centerY = (int)(primaryScreenLocation.ScaledSize.Height - ApplicationWindowConstants.DesignHeight) / 2;

                var desiredWindowBounds = new RectInt32(
                    _X: centerX,
                    _Y: centerY,
                    _Width: ApplicationWindowConstants.DesignWidth,
                    _Height: ApplicationWindowConstants.DesignHeight);

                MainWindow.AppWindow.MoveAndResize(desiredWindowBounds);
            }
        }
    }
}
