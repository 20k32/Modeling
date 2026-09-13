using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Providers;
using Modeling.Core.Constants;
using Modeling.Core.Serializer;
using Modeling.Core.Settings;
using System.Threading.Tasks;

namespace Modeling.Core.Drawing.Providers
{
    sealed class DrawingSettingsProvider : IDrawingSettingsProvider
    {
        readonly IApplicationSettingsProvider _applicationSettingsProvider;
        readonly IApplicationKeyProvider _applicationKeyProvider;
        readonly IStorageItemProvider _storageItemProvider;
        readonly ISerializer _serializer;

        string _storageItemFileToken;

        public IDrawingSettings Settings { get; private set; }

        public DrawingSettingsProvider()
        {
            _applicationSettingsProvider = Ioc.Default.GetService<IApplicationSettingsProvider>();
            _applicationKeyProvider = Ioc.Default.GetService<IApplicationKeyProvider>();
            _storageItemProvider = Ioc.Default.GetService<IStorageItemProvider>();
            _serializer = Ioc.Default.GetService<ISerializer>();
        }

        public async Task LoadSettingsAsync()
        {
            var fileContent = await _storageItemProvider.LoadContentAsync();
            Settings = _serializer.DeserializeFromString<IDrawingSettings>(fileContent) 
                ?? Ioc.Default.GetRequiredService<IDrawingSettings>();
        }

        public async Task SaveSettingsAsync()
        {
            var serializedContent = _serializer.Serialize(Settings);
            await _storageItemProvider.SaveContentAsync(serializedContent);
        }

        public async Task InitializeAsync()
        {
            await _applicationKeyProvider.InitializeAsync();
            await _applicationSettingsProvider.InitializeAsync(CoreConstants.APPLICATION_SETTINGS_DEFAULT_FILE_NAME_WITH_EXTENSION);

            var key = _applicationKeyProvider.DrawingSettingsTokenKey;

            _storageItemFileToken = _applicationSettingsProvider.GetSettingsValue<string>(key);

            await _storageItemProvider.InitializeAsync(key, _storageItemFileToken, CoreConstants.SAVING_FILE_NAME_WITH_EXTENSION);
        }

        public void SetFileToken(string token)
        {
            var key = _applicationKeyProvider.DrawingSettingsTokenKey;

            _applicationSettingsProvider.SetSettingsValue(key, token);
        }
    }
}
