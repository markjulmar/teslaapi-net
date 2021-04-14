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
        /// Current driver temperature setting in celsius.
        /// </summary>
        [JsonPropertyName("driver_temp_setting")]
        public double DriverTempSetting { get; set; }

        /// <summary>
        /// Current fan level.
        /// 0 = off, 1-5
        /// </summary>
        [JsonPropertyName("fan_status")]
        public int FanStatus { get; set; }

        /// <summary>
        /// Car temperature in celsius
        /// </summary>
        [JsonPropertyName("inside_temp")]
        public double InsideTemp { get; set; }

        /// <summary>
        /// True if auto temp control is turned on.
        /// </summary>
        [JsonPropertyName("is_auto_conditioning_on")]
        public bool IsAutoConditioningOn { get; set; }

        /// <summary>
        /// True if the climate system is running.
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

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("left_temp_direction")]
        public int LeftTempDirection { get; set; }

        /// <summary>
        /// Max temp setting in celsius
        /// </summary>
        [JsonPropertyName("max_avail_temp")]
        public double MaxAvailTemp { get; set; }

        /// <summary>
        /// Min temp setting in celsius
        /// </summary>
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

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("right_temp_direction")]
        public int RightTempDirection { get; set; }

        /// <summary>
        /// Driver seat heater value
        /// </summary>
        [JsonPropertyName("seat_heater_left")]
        public SeatHeater DriverSeatHeater { get; set; }

        /// <summary>
        /// Passenger seat heater value
        /// </summary>
        [JsonPropertyName("seat_heater_right")]
        public SeatHeater PassengerSeatHeater { get; set; }

        /// <summary>
        /// Rear center seat heater
        /// </summary>
        [JsonPropertyName("seat_heater_rear_center")]
        public SeatHeater RearCenterSeatHeater { get; set; }

        /// <summary>
        /// Rear left seat heater
        /// </summary>
        [JsonPropertyName("seat_heater_rear_left")]
        public SeatHeater RearLeftSeatHeater { get; set; }

        /// <summary>
        /// Rear right seat heater
        /// </summary>
        [JsonPropertyName("seat_heater_rear_right")]
        public SeatHeater RearRightSeatHeater { get; set; }
        
        /// <summary>
        /// True if the mirror heater is turned on.
        /// </summary>
        [JsonPropertyName("side_mirror_heaters")]
        public bool SideMirrorHeaters { get; set; }

        /// <summary>
        /// Timestamp for this object
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// True if the wiper blade heaters are active.
        /// </summary>
        [JsonPropertyName("wiper_blade_heater")]
        public bool WiperBladeHeater { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() 
            => $"Climate On: {IsClimateOn}, Driver Temp: {DriverTempSetting}, Preconditioning: {IsPreconditioning}";
    }
}
