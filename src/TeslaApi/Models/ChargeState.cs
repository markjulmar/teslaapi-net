using System;
using System.Text.Json.Serialization;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// JSON object representing the charge status for a vehicle.
    /// </summary>
    public sealed class ChargeState
    {
        /// <summary>
        /// True if the battery heater is on.
        /// </summary>
        [JsonPropertyName("battery_heater_on")]
        public bool BatteryHeaterOn { get; set; }

        /// <summary>
        /// Current battery level % (0-100).
        /// </summary>
        [JsonPropertyName("battery_level")]
        public int BatteryLevel { get; set; }

        /// <summary>
        /// Current rated battery range in miles/km
        /// </summary>
        [JsonPropertyName("battery_range")]
        public double RatedBatteryRange { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("charge_current_request")]
        public int ChargeCurrentRequest { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("charge_current_request_max")]
        public int ChargeCurrentRequestMax { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("charge_enable_request")]
        public bool ChargeEnableRequest { get; set; }

        /// <summary>
        /// kW added in last charge
        /// </summary>
        [JsonPropertyName("charge_energy_added")]
        public double ChargeEnergyAdded { get; set; }

        /// <summary>
        /// Current charge limit
        /// </summary>
        [JsonPropertyName("charge_limit_soc")]
        public int ChargeLimitSoc { get; set; }

        [JsonPropertyName("charge_limit_soc_max")]
        public int ChargeLimitSocMax { get; set; }

        [JsonPropertyName("charge_limit_soc_min")]
        public int ChargeLimitSocMin { get; set; }

        [JsonPropertyName("charge_limit_soc_std")]
        public int ChargeLimitSocStd { get; set; }

        [JsonPropertyName("charge_miles_added_ideal")]
        public double ChargeMilesAddedIdeal { get; set; }

        [JsonPropertyName("charge_miles_added_rated")]
        public double ChargeMilesAddedRated { get; set; }

        [JsonPropertyName("charge_port_cold_weather_mode")]
        public bool? ChargePortColdWeatherMode { get; set; }

        [JsonPropertyName("charge_port_door_open")]
        public bool ChargePortDoorOpen { get; set; }

        /// <summary>
        /// Current status of the port latch
        /// "Engaged"
        /// </summary>
        [JsonPropertyName("charge_port_latch")]
        public string ChargePortLatch { get; set; }

        [JsonPropertyName("charge_rate")]
        public double ChargeRate { get; set; }

        [JsonPropertyName("charge_to_max_range")]
        public bool ChargeToMaxRange { get; set; }

        /// <summary>
        /// Actual current from the charger.
        /// </summary>
        [JsonPropertyName("charger_actual_current")]
        public int ChargerActualCurrent { get; set; }

        [JsonPropertyName("charger_phases")]
        public string ChargerPhases { get; set; }

        [JsonPropertyName("charger_pilot_current")]
        public int ChargerPilotCurrent { get; set; }

        [JsonPropertyName("charger_power")]
        public int ChargerPower { get; set; }

        /// <summary>
        /// Charger voltage (120V)
        /// </summary>
        [JsonPropertyName("charger_voltage")]
        public int ChargerVoltage { get; set; }

        /// <summary>
        /// Current charging state
        /// “Starting”, “Complete”, “Charging”, “Disconnected”, “Stopped”, “NoPower”
        /// </summary>
        [JsonPropertyName("charging_state")]
        public string ChargingState { get; set; }

        [JsonPropertyName("conn_charge_cable")]
        public string ConnChargeCable { get; set; }

        /// <summary>
        /// Estimated battery range based on current charge.
        /// </summary>
        [JsonPropertyName("est_battery_range")]
        public double EstBatteryRange { get; set; }

        /// <summary>
        /// Fast charger brand
        /// </summary>
        [JsonPropertyName("fast_charger_brand")]
        public string FastChargerBrand { get; set; }

        /// <summary>
        /// True if a fast charger is present.
        /// </summary>
        [JsonPropertyName("fast_charger_present")]
        public bool FastChargerPresent { get; set; }

        /// <summary>
        /// Fast charger type
        /// </summary>
        [JsonPropertyName("fast_charger_type")]
        public string FastChargerType { get; set; }

        /// <summary>
        /// Ideal battery range if driving conditions were perfect.
        /// </summary>
        [JsonPropertyName("ideal_battery_range")]
        public double IdealBatteryRange { get; set; }

        [JsonPropertyName("managed_charging_active")]
        public bool ManagedChargingActive { get; set; }

        [JsonPropertyName("managed_charging_start_time")]
        public DateTime? ManagedChargingStartTime { get; set; }

        [JsonPropertyName("managed_charging_user_canceled")]
        public bool ManagedChargingUserCanceled { get; set; }

        [JsonPropertyName("max_range_charge_counter")]
        public int MaxRangeChargeCounter { get; set; }

        /// <summary>
        /// Minutes left until a full charge.
        /// </summary>
        [JsonPropertyName("minutes_to_full_charge")]
        public int MinutesToFullCharge { get; set; }

        /// <summary>
        /// True indicates low power condition.
        /// </summary>
        [JsonPropertyName("not_enough_power_to_heat")]
        public bool? NotEnoughPowerToHeat { get; set; }

        [JsonPropertyName("scheduled_charging_pending")]
        public bool ScheduledChargingPending { get; set; }

        [JsonPropertyName("scheduled_charging_start_time")]
        public DateTime? ScheduledChargingStartTime { get; set; }

        /// <summary>
        /// Time left to a full charge
        /// </summary>
        [JsonPropertyName("time_to_full_charge")]
        public double TimeToFullCharge { get; set; }

        /// <summary>
        /// Timestamp when this object was created.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("trip_charging")]
        public bool TripCharging { get; set; }

        /// <summary>
        /// Current usable battery level %
        /// </summary>
        [JsonPropertyName("usable_battery_level")]
        public int UsableBatteryLevel { get; set; }

        [JsonPropertyName("user_charge_enable_request")]
        public string UserChargeEnableRequest { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() 
            => $"Battery Level: {BatteryLevel}, Charging State: {ChargingState}, Battery Range: {RatedBatteryRange}, Estimated Battery Range: {EstBatteryRange}";
    }
}
