using System.Text.Json.Serialization;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// DTO for the basic vehicle information
    /// </summary>
    public class VehicleInfo
    {
        /// <summary>
        /// Unique identifier on this account for this vehicle.
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }
        
        /// <summary>
        /// Vehicle Id from Tesla
        /// </summary>
        [JsonPropertyName("vehicle_id")]
        public long VehicleId { get; set; }
        
        /// <summary>
        /// Assigned VIN for the car.
        /// </summary>
        [JsonPropertyName("vin")]
        public string VIN { get; set; }
        
        /// <summary>
        /// Display name (assigned by owner)
        /// </summary>
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
        
        /// <summary>
        /// Option codes (not reliable)
        /// </summary>
        [JsonPropertyName("option_codes")]
        public string OptionCodes { get; set; }

        /// <summary>
        /// Color of vehicle. Not always present, use GetVehicleData to retrieve more details.
        /// </summary>
        [JsonPropertyName("color")]
        public string Color { get; set; }
        
        /// <summary>
        /// Current state of the car
        /// </summary>
        [JsonPropertyName("state")]
        public string State { get; set; }
        
        /// <summary>
        /// True if the car is in service.
        /// </summary>
        [JsonPropertyName("in_service")]
        public bool InService { get; set; }
        
        /// <summary>
        /// True if the calendar is enabled.
        /// </summary>
        [JsonPropertyName("calendar_enabled")]
        public bool CalendarEnabled { get; set; }
        
        /// <summary>
        /// API version
        /// </summary>
        [JsonPropertyName("api_version")]
        public int ApiVersion { get; set; }
        
        // TODO: the following fields are also present in the response, but I'm not sure what they
        // are used for. Leaving them out of the definition for now.
        //[JsonPropertyName("tokens")] public List<string> Tokens { get; set; }
        //[JsonPropertyName("id_s")] public string IdS { get; set; }
        //[JsonPropertyName("backseat_token")] public object BackseatToken { get; set; }
        //[JsonPropertyName("backseat_token_updated_at")] public object BackseatTokenUpdatedAt { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => $"Id={Id}, Vid={VehicleId}, Name={DisplayName}, VIN={VIN}, Color={Color}, State={State}";
    }
}