using Modeling.Core.Settings;
using Newtonsoft.Json;
using System;

namespace Modeling.Core.Serializer.NewtonSoft
{
    sealed class DrawingSettingsJsonConverter : JsonConverter<IDrawingSettings>
    {
        public override void WriteJson(
            JsonWriter writer,
            IDrawingSettings value,
            JsonSerializer serializer)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None
            };

            var typeSerializer = JsonSerializer.Create(serializerSettings);

            typeSerializer.Serialize(writer, value);
        }

        public override IDrawingSettings ReadJson(
            JsonReader reader,
            Type objectType,
            IDrawingSettings existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            return serializer.Deserialize<DrawingSettings>(reader);
        }
    }
}
