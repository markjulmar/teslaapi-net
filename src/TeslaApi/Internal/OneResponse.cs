using System.Text.Json.Serialization;

namespace Julmar.TeslaApi.Internal
{
    class OneResponse<T>
    {
        [JsonPropertyName("response")]
        public T Response { get; set; }
    }
}