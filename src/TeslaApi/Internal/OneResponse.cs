using System.Text.Json.Serialization;

namespace Julmar.TeslaApi.Internal
{
    /// <summary>
    /// Single object response from Tesla API
    /// </summary>
    /// <typeparam name="T">Object response type</typeparam>
    internal sealed class OneResponse<T>
    {
        /// <summary>
        /// Response
        /// </summary>
        [JsonPropertyName("response")]
        public T Response { get; set; }
    }
}