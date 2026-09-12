using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Providers;
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
            var fileContent = await _storageItemProvider.LoadContentAsync(_storageItemFileToken);
            Settings = _serializer.DeserializeFromString<IDrawingSettings>(fileContent);
        }

        public async Task SaveSettingsAsync()
        {
            var serializedContent = _serializer.Serialize(Settings);
            await _storageItemProvider.SaveContentAsync(serializedContent, _storageItemFileToken);
        }

        public async Task InitializeAsync()
        {
            await _applicationKeyProvider.InitializeAsync();

            var key = _applicationKeyProvider.DrawingSettingsTokenKey;

            _storageItemFileToken = await _applicationSettingsProvider.GetSettingsValueAsync<string>(key);
        }

        public async Task SetFileTokenAsync(string token)
        {
            var key = _applicationKeyProvider.DrawingSettingsTokenKey;

            await _applicationSettingsProvider.SetSettingsValueAsync(key, token);
        }
    }
}
