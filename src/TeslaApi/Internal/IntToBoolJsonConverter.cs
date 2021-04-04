using System;
using System.Data;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Julmar.TeslaApi.Internal
{
    /// <summary>
    /// Allow boolean representations from integer values.
    /// </summary>
    public class IntToBoolJsonConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                int value = reader.GetInt32();
                return value != 0;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                string value = reader.GetString();
                return bool.TryParse(value, out var result) && result;
            }

            return reader.GetBoolean();
        }
        
        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}