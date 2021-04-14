using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Julmar.TeslaApi.Internal
{
    /// <summary>
    /// Allow boolean representations from integer values.
    /// </summary>
    internal class IntToBoolJsonConverter : JsonConverter<bool>
    {
        /// <summary>Reads and converts the JSON to type <typeparamref name="T" />.</summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns>The converted value.</returns>
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    return reader.GetInt32() != 0;
                case JsonTokenType.String:
                    return bool.TryParse(reader.GetString(), out var result) && result;
                default:
                    return reader.GetBoolean();
            }
        }

        /// <summary>Writes a specified value as JSON.</summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}