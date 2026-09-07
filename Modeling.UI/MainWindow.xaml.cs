using WinUIEx;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using CommunityToolkit.WinUI.Media;

namespace Modeling.UI
{
    public sealed partial class MainWindow : WindowEx
    {
        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
        }
    }
}
