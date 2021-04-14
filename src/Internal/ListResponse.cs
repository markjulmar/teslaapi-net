using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Julmar.TeslaApi.Internal
{
    /// <summary>
    /// Response from API when a list is returned.
    /// </summary>
    /// <typeparam name="T">Response object type</typeparam>
    internal sealed class ListResponse<T>
    {
        /// <summary>
        /// List of objects
        /// </summary>
        [JsonPropertyName("response")]
        public List<T> Response { get; set; }

        /// <summary>
        /// Number of objects returned in list.
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}