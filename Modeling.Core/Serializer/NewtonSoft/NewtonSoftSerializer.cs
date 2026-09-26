using CommunityToolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;

namespace Modeling.Core.Serializer.NewtonSoft
{
    sealed class NewtonSoftSerializer : ISerializer
    {
        readonly JsonSerializerSettings _options;
        readonly JsonSerializerSettings _defaultOptions;

        public NewtonSoftSerializer()
        {
            _options = new()
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                ContractResolver = Ioc.Default.GetRequiredService<NewtonSoftSerializerContractResolver>()
            };

            _defaultOptions = new()
            {
                TypeNameHandling = TypeNameHandling.None,
                Formatting = Formatting.Indented,
                ContractResolver = Ioc.Default.GetRequiredService<NewtonSoftSerializerContractResolver>()
            };
        }

        public T DeserializeFromString<T>(string value, bool useSerializerSettings = true)
            => JsonConvert.DeserializeObject<T>(value, useSerializerSettings ? _options : _defaultOptions);

        public string Serialize<T>(T value, bool useSerializerSettings = true)
            => JsonConvert.SerializeObject(value, useSerializerSettings ? _options : _defaultOptions);
    }
}
