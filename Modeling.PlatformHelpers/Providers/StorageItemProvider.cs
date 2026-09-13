using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Abstractions.Providers;
using Modeling.Core.Logging;
using Modeling.PlatformHelpers.Miscellaneous;
using Modeling.PlatformHelpers.Providers.ApplicationData;
using Modeling.PlatformHelpers.Providers.FutureAccessList;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Modeling.PlatformHelpers.Providers
{
    sealed class StorageItemProvider : IStorageItemProvider
    {
        readonly IApplicationSettingsProvider _applicationSettingsProvider;
        readonly IApplicationDataProvider _applicationDataProvider;
        readonly IFutureAccessListProvider _futureAccessListProvider;

        IStorageFile _savingFile;

        public string PathToFile { get; private set; }

        public StorageItemProvider()
        {
            _applicationSettingsProvider = Ioc.Default.GetService<IApplicationSettingsProvider>();
            _applicationDataProvider = Ioc.Default.GetService<IApplicationDataProvider>();
            _futureAccessListProvider = Ioc.Default.GetService<IFutureAccessListProvider>();
        }

        async Task InitializeDefaultSavingFileAsync(string key, string defaultFileNameWithExtension)
        {
            try
            {
                _savingFile = await _applicationDataProvider.LocalFolder.CreateFileAsync(defaultFileNameWithExtension,
                    CreationCollisionOption.ReplaceExisting);

                var token = _applicationSettingsProvider.GetSettingsValue<string>(key);

                if (_futureAccessListProvider.ContainsItem(token))
                {
                    _futureAccessListProvider.Remove(token);
                }

                var newToken = _futureAccessListProvider.Add(_savingFile);

                _applicationSettingsProvider.SetSettingsValue(key, newToken);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public async Task InitializeAsync(string key, string token, string defaultFileNameWithExtension)
        {
            await _futureAccessListProvider.InitializeAsync(Constants.UNPACKAGED_APPLICATION_FUTURE_ACCES_LIST_FILE_NAME_WITH_EXTENSION);

            if (string.IsNullOrWhiteSpace(token))
            {
                await InitializeDefaultSavingFileAsync(key, defaultFileNameWithExtension);
            }
            else
            {
                await GetSavingFileFromFutureAccessList(token);

                if (_savingFile is null)
                {
                    await InitializeDefaultSavingFileAsync(key, defaultFileNameWithExtension);
                }
            }
        }

        public async Task GetSavingFileFromFutureAccessList(string token)
        {
            try
            {
                _savingFile = await _futureAccessListProvider.GetFileAsync(token);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }

        public async Task<string> LoadContentAsync() => await FileIO.ReadTextAsync(_savingFile);

        public async Task SaveContentAsync(string content) => await FileIO.WriteTextAsync(_savingFile, content);
    }
}
