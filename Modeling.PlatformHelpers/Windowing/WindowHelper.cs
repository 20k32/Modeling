using Microsoft.UI.Xaml;
using System;

namespace Modeling.PlatformHelpers.Windowing
{
    sealed class WindowHelper : IWindowHelper
    {
        private Window mainWindow;
        public Window MainWindow
        {
            get => mainWindow;
            set
            {
                if (mainWindow is not null)
                {
                    throw new ArgumentOutOfRangeException(string.Format(Constants.ArgumentNotNullExceptionFormat, mainWindow));
                }
                else if (value is null)
                {
                    throw new ArgumentNullException(string.Format(Constants.ArgumentNullExceptionFormat, mainWindow));
                }
                else
                {
                    mainWindow = value;
                }
            }
        }

        private bool CanChangeMainWindow => mainWindow is not null;

        public void CenterMainWindow()
        {
            if (CanChangeMainWindow)
            {

            }
        }
    }
}
