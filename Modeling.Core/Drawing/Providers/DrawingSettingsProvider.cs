using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Providers;
using Modeling.Core.Constants;
using Modeling.Core.Serializer;
using Modeling.Core.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Modeling.Core.Drawing.Providers
{
    sealed class DrawingSettingsProvider : IDrawingSettingsProvider
    {
        readonly SemaphoreSlim _settingsSavingLock;
        readonly IApplicationSettingsProvider _applicationSettingsProvider;
        readonly IApplicationKeyProvider _applicationKeyProvider;
        readonly IStorageItemProvider _storageItemProvider;
        readonly ISerializer _serializer;

        string _storageItemFileToken;

        public IDrawingSettings Settings { get; private set; }
        public bool ShouldInvokeSettingsChanged
        {
            get => Settings.ShouldInvokeSettingsChanged;
            set => Settings.ShouldInvokeSettingsChanged = value;
        }

        public event ActionEventHandler SettingsChanged;

        public DrawingSettingsProvider()
        {
            _settingsSavingLock = new SemaphoreSlim(1, 1);
            _applicationSettingsProvider = Ioc.Default.GetService<IApplicationSettingsProvider>();
            _applicationKeyProvider = Ioc.Default.GetService<IApplicationKeyProvider>();
            _storageItemProvider = Ioc.Default.GetService<IStorageItemProvider>();
            _serializer = Ioc.Default.GetService<ISerializer>();
        }

        public async Task LoadSettingsAsync()
        {
            var fileContent = await _storageItemProvider.LoadContentAsync();

            Settings?.SettingsChanged -= OnDrawingSettingsSettingsChanged;

            Settings = _serializer.DeserializeFromString<IDrawingSettings>(fileContent, useSerializerSettings: false)
                ?? Ioc.Default.GetRequiredService<IDrawingSettings>();

            Settings.SettingsChanged -= OnDrawingSettingsSettingsChanged;
            Settings.SettingsChanged += OnDrawingSettingsSettingsChanged;
        }

        private void OnDrawingSettingsSettingsChanged()
        {
            _ = SaveSettingsAsync();

            SettingsChanged?.Invoke();
        }

        public async Task SaveSettingsAsync()
        {
            try
            {
                await _settingsSavingLock.WaitAsync();

                var serializedContent = _serializer.Serialize(Settings, useSerializerSettings: false);
                await _storageItemProvider.SaveContentAsync(serializedContent);
            }
            finally
            {
                _settingsSavingLock.Release();
            }
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _settingsSavingLock.WaitAsync();

                await _applicationKeyProvider.InitializeAsync();
                await _applicationSettingsProvider.InitializeAsync(CoreConstants.APPLICATION_SETTINGS_DEFAULT_FILE_NAME_WITH_EXTENSION);

                var key = _applicationKeyProvider.DrawingSettingsTokenKey;

                _storageItemFileToken = _applicationSettingsProvider.GetSettingsValue<string>(key);

                await _storageItemProvider.InitializeAsync(key, _storageItemFileToken, CoreConstants.SAVING_FILE_NAME_WITH_EXTENSION);
            }
            finally
            {
                _settingsSavingLock.Release();
            }
        }

        public void SetFileToken(string token)
        {
            var key = _applicationKeyProvider.DrawingSettingsTokenKey;

            _applicationSettingsProvider.SetSettingsValue(key, token);
        }
    }
}
