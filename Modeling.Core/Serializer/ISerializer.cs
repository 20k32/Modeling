namespace Modeling.Core.Serializer
{
    public interface ISerializer
    {
        string Serialize<T>(T value);
        T DeserializeFromString<T>(string value);
    }
}
