namespace Modeling.Core.Serializer
{
    public interface ISerializer
    {
        string Serialize<T>(T value, bool useSerializerSettings = true);
        T DeserializeFromString<T>(string value, bool useSerializerSettings = true);
    }
}
