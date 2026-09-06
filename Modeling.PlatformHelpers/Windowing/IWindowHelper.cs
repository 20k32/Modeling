using Microsoft.UI.Xaml;

namespace Modeling.PlatformHelpers.Windowing
{
    public interface IWindowHelper
    {
        public Window MainWindow { get; set; }
        public void CenterMainWindow();
        public void ActivateApplicationWindow();
    }
}
