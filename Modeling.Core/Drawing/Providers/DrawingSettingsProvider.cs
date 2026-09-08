using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Providers;
using Modeling.Core.Settings;
using System;
using System.Threading.Tasks;

namespace Modeling.Core.Drawing.Providers
{
    //todo: fix formatting in whole app
    //todo: json parsing
    sealed class DrawingSettingsProvider : IDrawingSettingsProvider
    {
        IApplicationSettingsProvider _applicationSettingsProvider;
        IApplicationKeyProvider _applicationKeyProvider;
        IStorageItemProvider _storageItemProvider;

        string _storageItemFileToken;

        public IDrawingSettings Settings { get; }

        public DrawingSettingsProvider()
        {
            _applicationSettingsProvider = Ioc.Default.GetService<IApplicationSettingsProvider>();
            _applicationKeyProvider = Ioc.Default.GetService<IApplicationKeyProvider>();
            _storageItemProvider = Ioc.Default.GetService<IStorageItemProvider>();
        }

        public async Task LoadSettingsAsync()
        {
            var fileContent = await _storageItemProvider.LoadContentAsync(_storageItemFileToken);
        }

        public async Task SaveSettingsAsync()
        {
            var serializedContent = string.Empty;
            await _storageItemProvider.SaveContentAsync(serializedContent, _storageItemFileToken);
        }

        public async Task InitializeAsync()
        {
            await _applicationKeyProvider.InitializeAsync();

            var key = _applicationKeyProvider.DrawingSettingsTokenKey;

            _storageItemFileToken = await _applicationSettingsProvider.GetSettingsValueAsync<string>(key);
        }
    }
}
