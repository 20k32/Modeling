using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Modeling.Core.Abstractions.Dialogs;
using Modeling.Models.UserInterface;
using System;
using System.Threading.Tasks;

namespace Modeling.UI.Resources.Dialogs
{
    public abstract partial class OnTopDialog : UserControl, IClosableDialog
    {
        readonly UIElement _rootContent;
        readonly IUserInterfaceConstantsProvider _userInterfaceConstantsProvider;

        protected Panel DialogContainer { get; private set; }

        protected OnTopDialog(UIElement rootContent)
        {
            _rootContent = rootContent;
            _userInterfaceConstantsProvider = Ioc.Default.GetRequiredService<IUserInterfaceConstantsProvider>();

            DialogContainer = _rootContent.FindDescendant<Panel>(panel
                => panel.Tag as string == _userInterfaceConstantsProvider.ApplicationPopupRootName)!;
        }

        [RelayCommand]
        void CloseDialog()
        {
            Close();
            DialogContainer.Visibility = Visibility.Collapsed;
        }

        protected virtual void CloseCore()
        {
            DialogContainer.Children.Remove(this);
        }

        protected virtual void ShowCore()
        {
            if (DialogContainer.Visibility != Visibility.Visible)
            {
                DialogContainer.Visibility = Visibility.Visible;
            }

            DialogContainer.Children.Add(this);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public void Close()
        {
            CloseCore();
        }

        public Task CloseAsync()
        {
            CloseCore();

            return Task.CompletedTask;
        }

        public void Show()
        {
            ShowCore();
        }

        public Task ShowAsync()
        {
            ShowCore();

            return Task.CompletedTask;
        }
    }
}
