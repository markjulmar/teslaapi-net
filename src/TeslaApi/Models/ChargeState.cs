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
        /// Requested AMPs based on session negotiation
        /// </summary>
        [JsonPropertyName("charge_current_request")]
        public int ChargeCurrentRequest { get; set; }

        /// <summary>
        /// Max AMPs requested based on session negotiation
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
        /// Current charge limit % (90)
        /// </summary>
        [JsonPropertyName("charge_limit_soc")]
        public int ChargeLimitSoc { get; set; }

        /// <summary>
        /// Charge limit max % (100)
        /// </summary>
        [JsonPropertyName("charge_limit_soc_max")]
        public int ChargeLimitSocMax { get; set; }

        /// <summary>
        /// Charge limit min % (50)
        /// </summary>
        [JsonPropertyName("charge_limit_soc_min")]
        public int ChargeLimitSocMin { get; set; }

        /// <summary>
        /// Charge limit standard % (90)
        /// </summary>
        [JsonPropertyName("charge_limit_soc_std")]
        public int ChargeLimitSocStd { get; set; }

        /// <summary>
        /// Ideal number of miles added so far for this session.
        /// </summary>
        [JsonPropertyName("charge_miles_added_ideal")]
        public double ChargeMilesAddedIdeal { get; set; }

        /// <summary>
        /// Actual number of miles added so far for this session.
        /// </summary>
        [JsonPropertyName("charge_miles_added_rated")]
        public double ChargeMilesAddedRated { get; set; }

        /// <summary>
        /// True if the charging port is in cold weather mode.
        /// </summary>
        [JsonPropertyName("charge_port_cold_weather_mode")]
        public bool? ChargePortColdWeatherMode { get; set; }

        /// <summary>
        /// True if the charging port is open.
        /// </summary>
        [JsonPropertyName("charge_port_door_open")]
        public bool ChargePortDoorOpen { get; set; }

        /// <summary>
        /// Current status of the charging port latch (lock on charging cord)
        /// "Engaged", "Disengaged"
        /// </summary>
        [JsonPropertyName("charge_port_latch")]
        public string ChargePortLatch { get; set; }

        /// <summary>
        /// How many miles/km per hour being added on current charging session.
        /// </summary>
        [JsonPropertyName("charge_rate")]
        public double ChargeRate { get; set; }

        /// <summary>
        /// True if this session will charge to the maximum range.
        /// </summary>
        [JsonPropertyName("charge_to_max_range")]
        public bool ChargeToMaxRange { get; set; }

        /// <summary>
        /// Amps current from the charger.
        /// </summary>
        [JsonPropertyName("charger_actual_current")]
        public int ChargerActualCurrent { get; set; }

        /// <summary>
        /// Current phase for charger
        /// </summary>
        [JsonPropertyName("charger_phases")]
        public int? ChargerPhases { get; set; }

        /// <summary>
        /// Amps from the charger
        /// </summary>
        [JsonPropertyName("charger_pilot_current")]
        public int ChargerPilotCurrent { get; set; }

        /// <summary>
        /// Kw for this charging session
        /// </summary>
        [JsonPropertyName("charger_power")]
        public int ChargerPower { get; set; }

        /// <summary>
        /// Charger voltage (120, 240, etc.)
        /// </summary>
        [JsonPropertyName("charger_voltage")]
        public int ChargerVoltage { get; set; }

        /// <summary>
        /// Current charging state
        /// “Starting”, “Complete”, “Charging”, “Disconnected”, “Stopped”, “NoPower”
        /// </summary>
        [JsonPropertyName("charging_state")]
        public string ChargingState { get; set; }

        /// <summary>
        /// Type of charging cable connected
        /// "SAE"
        /// </summary>
        [JsonPropertyName("conn_charge_cable")]
        public string ConnChargeCable { get; set; }

        /// <summary>
        /// Estimated battery range based on current charge.
        /// </summary>
        [JsonPropertyName("est_battery_range")]
        public double EstBatteryRange { get; set; }

        /// <summary>
        /// Fast charger brand
        /// "invalid"
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
        /// "MCSingleWireCAN"
        /// </summary>
        [JsonPropertyName("fast_charger_type")]
        public string FastChargerType { get; set; }

        /// <summary>
        /// Ideal battery range if driving conditions were perfect.
        /// </summary>
        [JsonPropertyName("ideal_battery_range")]
        public double IdealBatteryRange { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("managed_charging_active")]
        public bool ManagedChargingActive { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("managed_charging_start_time")]
        public DateTime? ManagedChargingStartTime { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("managed_charging_user_canceled")]
        public bool ManagedChargingUserCanceled { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
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

        /// <summary>
        /// True if a scheduled charge is coming up.
        /// </summary>
        [JsonPropertyName("scheduled_charging_pending")]
        public bool ScheduledChargingPending { get; set; }

        /// <summary>
        /// Time for next scheduled charge, null if none. 
        /// </summary>
        [JsonPropertyName("scheduled_charging_start_time")]
        public DateTime? ScheduledChargingStartTime { get; set; }

        /// <summary>
        /// Time left to a full charge in hours.minutes.
        /// </summary>
        [JsonPropertyName("time_to_full_charge")]
        public double TimeToFullCharge { get; set; }

        /// <summary>
        /// Timestamp when this object was created.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// True if this is a full trip charge.
        /// </summary>
        [JsonPropertyName("trip_charging")]
        public bool TripCharging { get; set; }

        /// <summary>
        /// Current usable battery level %
        /// </summary>
        [JsonPropertyName("usable_battery_level")]
        public int UsableBatteryLevel { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("user_charge_enable_request")]
        public string UserChargeEnableRequest { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() 
            => $"Battery Level: {BatteryLevel}, Charging State: {ChargingState}, Battery Range: {RatedBatteryRange}, Estimated Battery Range: {EstBatteryRange}";
    }
}
