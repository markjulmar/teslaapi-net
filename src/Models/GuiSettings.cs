using System.Text.Json.Serialization;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// This JSON object returns how the UX is configured.
    /// You can use the UnitConverter class to do some conversions on values.
    /// </summary>
    public sealed class GuiSettings
    {
        /// <summary>
        /// True to use 24-hour time.
        /// </summary>
        [JsonPropertyName("gui_24_hour_time")]
        public bool Use24HourTime { get; set; }

        /// <summary>
        /// Charging rate units
        /// </summary>
        [JsonPropertyName("gui_charge_rate_units")]
        public string ChargeRateUnits { get; set; }

        /// <summary>
        /// Distance units
        /// </summary>
        [JsonPropertyName("gui_distance_units")]
        public string DistanceUnits { get; set; }

        /// <summary>
        /// Range units
        /// </summary>
        [JsonPropertyName("gui_range_display")]
        public string RangeDisplay { get; set; }

        /// <summary>
        /// Temperature units
        /// </summary>
        [JsonPropertyName("gui_temperature_units")]
        public string TemperatureUnits { get; set; }

        /// <summary>
        /// True to show range units
        /// </summary>
        [JsonPropertyName("show_range_units")]
        public bool ShowRangeUnits { get; set; }

        /// <summary>
        /// Timestamp of object.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() 
            => $"24hr: {Use24HourTime}, ChargeUnits: {ChargeRateUnits}, DistanceUnits: {DistanceUnits}";
    }
}
