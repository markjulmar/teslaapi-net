using System.Text.Json.Serialization;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// JSON object returned for /api/1/vehicles/{id}/data_request/climate_stat
    /// </summary>
    public sealed class ClimateState
    {
        /// <summary>
        /// True if the battery heater is running.
        /// </summary>
        [JsonPropertyName("battery_heater")]
        public bool BatteryHeater { get; set; }

        /// <summary>
        /// True if there is not enough power to run the battery heater.
        /// </summary>
        [JsonPropertyName("battery_heater_no_power")]
        public bool? BatteryHeaterNoPower { get; set; }

        /// <summary>
        /// True if auto-climate is turned on.
        /// </summary>
        [JsonPropertyName("climate_keeper_mode")]
        public string ClimateKeeperMode { get; set; }

        /// <summary>
        /// True if defrost is turned on.
        /// </summary>
        [JsonPropertyName("defrost_mode")]
        public int DefrostMode { get; set; }

        /// <summary>
        /// Current driver temperature setting.
        /// </summary>
        [JsonPropertyName("driver_temp_setting")]
        public double DriverTempSetting { get; set; }

        /// <summary>
        /// Current fan level.
        /// </summary>
        [JsonPropertyName("fan_status")]
        public int FanStatus { get; set; }

        /// <summary>
        /// Car temperature in celsius or fahrenheit
        /// </summary>
        [JsonPropertyName("inside_temp")]
        public double InsideTemp { get; set; }

        /// <summary>
        /// True if auto conditioning is turned on.
        /// </summary>
        [JsonPropertyName("is_auto_conditioning_on")]
        public bool IsAutoConditioningOn { get; set; }

        /// <summary>
        /// True if the climate system is on.
        /// </summary>
        [JsonPropertyName("is_climate_on")]
        public bool IsClimateOn { get; set; }

        /// <summary>
        /// True for front defroster.
        /// </summary>
        [JsonPropertyName("is_front_defroster_on")]
        public bool IsFrontDefrosterOn { get; set; }

        /// <summary>
        /// True if we are preconditioning the battery.
        /// </summary>
        [JsonPropertyName("is_preconditioning")]
        public bool IsPreconditioning { get; set; }

        /// <summary>
        /// True if the rear defroster is turned on.
        /// </summary>
        [JsonPropertyName("is_rear_defroster_on")]
        public bool IsRearDefrosterOn { get; set; }

        [JsonPropertyName("left_temp_direction")]
        public int LeftTempDirection { get; set; }

        [JsonPropertyName("max_avail_temp")]
        public double MaxAvailTemp { get; set; }

        [JsonPropertyName("min_avail_temp")]
        public double MinAvailTemp { get; set; }

        /// <summary>
        /// Current outside temperature
        /// </summary>
        [JsonPropertyName("outside_temp")]
        public double OutsideTemp { get; set; }

        /// <summary>
        /// Passenger side temperature setting.
        /// </summary>
        [JsonPropertyName("passenger_temp_setting")]
        public double PassengerTempSetting { get; set; }

        /// <summary>
        /// True if the remote heater control is turned on.
        /// </summary>
        [JsonPropertyName("remote_heater_control_enabled")]
        public bool RemoteHeaterControlEnabled { get; set; }

        [JsonPropertyName("right_temp_direction")]
        public int RightTempDirection { get; set; }

        [JsonPropertyName("seat_heater_left")]
        public int SeatHeaterLeft { get; set; }

        [JsonPropertyName("seat_heater_right")]
        public int SeatHeaterRight { get; set; }

        [JsonPropertyName("side_mirror_heaters")]
        public bool SideMirrorHeaters { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("wiper_blade_heater")]
        public bool WiperBladeHeater { get; set; }

        public override string ToString()
        {
            return $"Climate On: {IsClimateOn}, Driver Temp: {DriverTempSetting}, Preconditioning: {IsPreconditioning}";
        }
    }
}
