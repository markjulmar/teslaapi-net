using System.Text.Json.Serialization;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// This JSON object is returned from the /api/1/vehicles/{id}/vehicle_data API
    /// </summary>
    public sealed class VehicleDataRollup : VehicleInfo
    {
        /// <summary>
        /// Type of token used to retrieve the vehicle data
        /// "OWNER"
        /// </summary>
        [JsonPropertyName("access_type")]
        public string AccessType { get; set; }

        /// <summary>
        /// Current drive status
        /// </summary>
        [JsonPropertyName("drive_state")]
        public DriveState DriveState { get; set; }

        /// <summary>
        /// Current climate status
        /// </summary>
        [JsonPropertyName("climate_state")]
        public ClimateState ClimateState { get; set; }

        /// <summary>
        /// Current charge status
        /// </summary>
        [JsonPropertyName("charge_state")]
        public ChargeState ChargeState { get; set; }

        /// <summary>
        /// Current GUI settings
        /// </summary>
        [JsonPropertyName("gui_settings")]
        public GuiSettings GuiSettings { get; set; }

        /// <summary>
        /// Current vehicle status
        /// </summary>
        [JsonPropertyName("vehicle_state")]
        public VehicleState VehicleState { get; set; }

        /// <summary>
        /// Current vehicle configuration
        /// </summary>
        [JsonPropertyName("vehicle_config")]
        public VehicleConfiguration VehicleConfig { get; set; }
    }
}