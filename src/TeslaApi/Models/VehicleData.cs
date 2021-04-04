using System.Text.Json.Serialization;

namespace Julmar.TeslaApi
{
    public sealed class VehicleDataRollup : VehicleInfo
    {
        [JsonPropertyName("access_type")]
        public string AccessType { get; set; }

        [JsonPropertyName("drive_state")]
        public DriveState DriveState { get; set; }

        [JsonPropertyName("climate_state")]
        public ClimateState ClimateState { get; set; }

        [JsonPropertyName("charge_state")]
        public ChargeState ChargeState { get; set; }

        [JsonPropertyName("gui_settings")]
        public GuiSettings GuiSettings { get; set; }

        [JsonPropertyName("vehicle_state")]
        public VehicleState VehicleState { get; set; }

        [JsonPropertyName("vehicle_config")]
        public VehicleConfiguration VehicleConfig { get; set; }
    }
}