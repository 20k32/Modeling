using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Modeling.Core.Messages.Settings;
using Modeling.Core.Extensions;
using System.Threading.Tasks;
using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Drawing.Providers;

namespace Modeling.ViewModels
{
    public sealed partial class SettingsViewModel : ObservableObject
    {
        readonly IDrawingSettingsProvider _drawingSettingsProvider;

        bool _initialized;

        public SettingsViewModel()
        {
            _drawingSettingsProvider = Ioc.Default.GetService<IDrawingSettingsProvider>();
        }

        [RelayCommand]
        void Initialize()
        {
            WeakReferenceMessenger.Default.Register<InitializeSettingsMessage>(this, OnSettingsViewModelInitializeSettingsMessage);
        }

        void OnSettingsViewModelInitializeSettingsMessage(object recipient, InitializeSettingsMessage message)
        {
            if (!_initialized && message.ApplyBasicMessageValidation(recipient))
            {
                _initialized = true;

                RegisterHandlers();
            }
        }

        void RegisterHandlers()
        {
            WeakReferenceMessenger.Default.Register<SaveSettingsAsyncMessage>(this, OnSettingsViewModelSaveSettingsAsync);
            WeakReferenceMessenger.Default.Register<LoadSettingsAsyncMessage>(this, OnSettingsViewModelLoadSettingsAsync);
        }

        void OnSettingsViewModelLoadSettingsAsync(object recipient, LoadSettingsAsyncMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                message.Reply(LoadSettingsAsync());
            }
        }

        void OnSettingsViewModelSaveSettingsAsync(object recipient, SaveSettingsAsyncMessage message)
        {
            if (message.ApplyBasicMessageValidation(recipient))
            {
                message.Reply(SaveSettingsAsync());
            }
        }

        async Task LoadSettingsAsync()
        {
            await _drawingSettingsProvider.InitializeAsync();

            await _drawingSettingsProvider.LoadSettingsAsync();
        }

        async Task SaveSettingsAsync()
        {

        }
    }
}
