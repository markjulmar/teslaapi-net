using System.Text.Json.Serialization;

namespace TeslaApi
{
    public class GuiSettings
    {
        [JsonPropertyName("gui_24_hour_time")]
        public bool Use24HourTime { get; set; }

        [JsonPropertyName("gui_charge_rate_units")]
        public string ChargeRateUnits { get; set; }

        [JsonPropertyName("gui_distance_units")]
        public string DistanceUnits { get; set; }

        [JsonPropertyName("gui_range_display")]
        public string RangeDisplay { get; set; }

        [JsonPropertyName("gui_temperature_units")]
        public string TemperatureUnits { get; set; }

        [JsonPropertyName("show_range_units")]
        public bool ShowRangeUnits { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        public override string ToString()
        {
            return $"24hr: {Use24HourTime}, ChargeUnits: {ChargeRateUnits}, DistanceUnits: {DistanceUnits}";
        }
    }
}
