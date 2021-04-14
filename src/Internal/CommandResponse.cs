using System.Text.Json.Serialization;

namespace Julmar.TeslaApi.Internal
{
    /// <summary>
    /// JSON response packet to a /api/command
    /// </summary>
    internal sealed class CommandResponse
    {
        /// <summary>
        /// Reason text - seems to always be blank
        /// </summary>
        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        /// <summary>
        /// Boolean result
        /// </summary>
        [JsonPropertyName("result")]
        public bool Result { get; set; }
    }
}