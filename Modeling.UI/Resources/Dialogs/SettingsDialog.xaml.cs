using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Modeling.Core.Dialogs;
using Modeling.Models.Abstractions.Dialogs;
using Modeling.PlatformHelpers.Windowing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;


namespace Modeling.UI.Resources.Dialogs
{
    public sealed partial class SettingsDialog : OnTopDialog, ISettingsDialog
    {
        private DialogResult _dialogResult;

        public SettingsDialog() : base(Ioc.Default.GetRequiredService<IWindowHelper>().MainWindow.Content)
        {
            InitializeComponent();

            _dialogResult = DialogResult.Default;
        }

        public DialogResult DialogResult => _dialogResult;

        protected override void CloseCore()
        {
            _dialogResult = new DialogResult(DialogAction.Close);
            base.CloseCore();
        }
    }
}
