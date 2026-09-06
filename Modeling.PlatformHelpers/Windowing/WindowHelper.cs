using Microsoft.UI.Xaml;
using System;

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

            }
        }
    }
}
