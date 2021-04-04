using System.Text.Json.Serialization;

namespace Julmar.TeslaApi.Internal
{
    class CommandResponse
    {
        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("result")]
        public bool Result { get; set; }
    }
}