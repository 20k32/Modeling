using Newtonsoft.Json;

namespace Modeling.Core.Serializer
{
    sealed class NewtonSoftSerializer : ISerializer
    {
        readonly JsonSerializerSettings _options = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented
        };

        public T DeserializeFromString<T>(string value) => JsonConvert.DeserializeObject<T>(value, _options);

        public string Serialize<T>(T value) => JsonConvert.SerializeObject(value, _options);
    }
}
