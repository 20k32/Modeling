using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Modeling.Core.Messages.Settings;
using Modeling.Models.Abstractions.Dialogs;

namespace Modeling.ViewModels.Pages.MainPage.Settings
{
    public sealed partial class SettingsViewModel : BaseViewModel
    {
        [RelayCommand]
        void Initialize()
        {
            WeakReferenceMessenger.Default.Register<InitializeSettingsMessage>(this, OnSettingsViewModelInitializeSettingsMessage);
        }

        [RelayCommand]
        void ShowSettingsDialog()
        {
            var settingsDialog = Ioc.Default.GetRequiredService<ISettingsDialog>();
            settingsDialog.Show();
        }
    }
}
