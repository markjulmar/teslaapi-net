using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Julmar.TeslaApi
{
    public class Vehicle
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("vehicle_id")]
        public long VehicleId { get; set; }

        [JsonPropertyName("vin")]
        public string Vin { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("option_codes")]
        public string OptionCodes { get; set; }

        [JsonPropertyName("color")]
        public object Color { get; set; }

        [JsonPropertyName("tokens")]
        public List<string> Tokens { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("in_service")]
        public bool InService { get; set; }

        [JsonPropertyName("id_s")]
        public string IdS { get; set; }

        [JsonPropertyName("calendar_enabled")]
        public bool CalendarEnabled { get; set; }

        [JsonPropertyName("api_version")]
        public int ApiVersion { get; set; }

        [JsonPropertyName("backseat_token")]
        public object BackseatToken { get; set; }

        [JsonPropertyName("backseat_token_updated_at")]
        public object BackseatTokenUpdatedAt { get; set; }

        public override string ToString()
        {
            return $"Id={Id}, Vid={VehicleId}, Name={DisplayName}, VIN={Vin}, Color={Color}, State={State}";
        }
    }

    class OneResponse<T>
    {
        [JsonPropertyName("response")]
        public T Response { get; set; }
    }

    class ListResponse<T>
    {
        [JsonPropertyName("response")]
        public List<T> Response { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
