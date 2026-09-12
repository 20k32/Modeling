using CommunityToolkit.Mvvm.DependencyInjection;
using Modeling.Core.Serializer;
using Modeling.PlatformHelpers.Miscellaneous;
using Modeling.PlatformHelpers.Windowing;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Modeling.PlatformHelpers.Providers.PackageRelatedProviders.ApplicationDataAccess
{
    sealed class UnpackagedApplicationDataProvider : IPackageRelatedApplicationDataProvider
    {
        readonly object _settingsFileOperationsLock = new();
        readonly IWindowHelper _windowHelper;
        readonly ISerializer _serializer;
        readonly Dictionary<string, object> _settingsContainer;

        string _pathToSettingsFile;
        Microsoft.Windows.Storage.ApplicationData _applicationData;

        public UnpackagedApplicationDataProvider()
        {
            _serializer = Ioc.Default.GetService<ISerializer>();
            _settingsContainer = [];
            _windowHelper = Ioc.Default.GetService<IWindowHelper>();
        }

        public StorageFolder LocalFolder => _applicationData.LocalFolder;

        public object GetSettingsValue(string key)
        {
            _ = _settingsContainer.TryGetValue(key, out var result);

            return result;
        }

        public async Task InitializeAsync(string defaultFileNameWithExtension)
        {
            await _windowHelper.WindowInitializationTask;

            InitializeApplicationDataContainer();

            InitializeSettingsFile(defaultFileNameWithExtension);

            lock (_settingsFileOperationsLock)
            {
                InitializeSettingsContainer();
            }
        }

        public void SetSettingsValue(string key, object value)
        {
            lock (_settingsFileOperationsLock)
            {
                _settingsContainer[key] = value;

                var serializedSettings = _serializer.Serialize(_settingsContainer);

                File.WriteAllText(_pathToSettingsFile, serializedSettings);
            }
        }

        void InitializeApplicationDataContainer()
        {
            _applicationData = Microsoft.Windows.Storage.ApplicationData.GetForUnpackaged(Constants.UNPACKAGED_APPLICATION_PUBLISHER,
                Constants.UNPACKAGED_APPLICATION_PRODUCT_NAME);

            if (!Directory.Exists(_applicationData.LocalPath))
            {
                Directory.CreateDirectory(_applicationData.LocalPath);
            }

            if (!Directory.Exists(_applicationData.TemporaryPath))
            {
                Directory.CreateDirectory(_applicationData.TemporaryPath);
            }
        }

        void InitializeSettingsFile(string fileName)
        {
            var pathToApplicationDataFolder = _applicationData.LocalPath;
            var pathToSettingsDirectory = Path.Combine(pathToApplicationDataFolder,
                Constants.UNPACKAGED_APPLICATION_SETTINGS_FOLDER_NAME);

            if (!Directory.Exists(pathToSettingsDirectory))
            {
                Directory.CreateDirectory(pathToSettingsDirectory);
            }

            _pathToSettingsFile = Path.Combine(pathToSettingsDirectory, fileName);

            if (!File.Exists(_pathToSettingsFile))
            {
                var stream = File.Create(_pathToSettingsFile);
                stream.Dispose();
            }
        }

        void InitializeSettingsContainer()
        {
            _settingsContainer.Clear();

            var serializedSettingsValues = File.ReadAllText(_pathToSettingsFile);

            if ((serializedSettingsValues?.Length ?? 0) > 0)
            {
                var deserializedSettings = _serializer.DeserializeFromString<Dictionary<string, object>>(serializedSettingsValues);

                if (deserializedSettings is not null)
                {
                    foreach (var keyValue in deserializedSettings)
                    {
                        _settingsContainer.Add(keyValue.Key, keyValue.Value);
                    }
                }
            }
        }

        public void Remove(string key)
        {
            lock (_settingsFileOperationsLock)
            {
                _ = _settingsContainer.Remove(key);

                var serializedSettings = _serializer.Serialize(_settingsContainer);

                File.WriteAllText(_pathToSettingsFile, serializedSettings);
            }
        }

        public bool ContainsKey(string key) => _settingsContainer.ContainsKey(key);
    }
}
