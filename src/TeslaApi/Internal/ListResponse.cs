using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Julmar.TeslaApi.Internal
{
    class ListResponse<T>
    {
        [JsonPropertyName("response")]
        public List<T> Response { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}