using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Julmar.TeslaApi.Internal
{
    /// <summary>
    /// JSON converter to take an integer timestamp and turn it into a UTC date/time.
    /// </summary>
    internal class TimestampToDateTimeConverter : JsonConverter<DateTime?>
    {
        /// <summary>Reads and converts the JSON to type <typeparamref name="T" />.</summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert">The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns>The converted value.</returns>
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            long timestamp;
            switch (reader.TokenType)
            {
                case JsonTokenType.Number:
                    timestamp = reader.GetInt64();
                    break;
                case JsonTokenType.String:
                    Int64.TryParse(reader.GetString(), out timestamp);
                    break;
                default:
                    return null;
            }

            return DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
        }

        /// <summary>Writes a specified value as JSON.</summary>
        /// <param name="writer">The writer to write to.</param>
        /// <param name="value">The value to convert to JSON.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}