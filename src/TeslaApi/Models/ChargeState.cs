using System;
using System.Text.Json.Serialization;

namespace Julmar.TeslaApi
{
    public class ChargeState
    {
        [JsonPropertyName("battery_heater_on")]
        public bool BatteryHeaterOn { get; set; }

        [JsonPropertyName("battery_level")]
        public int BatteryLevel { get; set; }

        [JsonPropertyName("battery_range")]
        public double BatteryRange { get; set; }

        [JsonPropertyName("charge_current_request")]
        public int ChargeCurrentRequest { get; set; }

        [JsonPropertyName("charge_current_request_max")]
        public int ChargeCurrentRequestMax { get; set; }

        [JsonPropertyName("charge_enable_request")]
        public bool ChargeEnableRequest { get; set; }

        [JsonPropertyName("charge_energy_added")]
        public double ChargeEnergyAdded { get; set; }

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

        [JsonPropertyName("charge_port_latch")]
        public string ChargePortLatch { get; set; }

        [JsonPropertyName("charge_rate")]
        public double ChargeRate { get; set; }

        [JsonPropertyName("charge_to_max_range")]
        public bool ChargeToMaxRange { get; set; }

        [JsonPropertyName("charger_actual_current")]
        public int ChargerActualCurrent { get; set; }

        [JsonPropertyName("charger_phases")]
        public string ChargerPhases { get; set; }

        [JsonPropertyName("charger_pilot_current")]
        public int ChargerPilotCurrent { get; set; }

        [JsonPropertyName("charger_power")]
        public int ChargerPower { get; set; }

        [JsonPropertyName("charger_voltage")]
        public int ChargerVoltage { get; set; }

        [JsonPropertyName("charging_state")]
        public string ChargingState { get; set; }

        [JsonPropertyName("conn_charge_cable")]
        public string ConnChargeCable { get; set; }

        [JsonPropertyName("est_battery_range")]
        public double EstBatteryRange { get; set; }

        [JsonPropertyName("fast_charger_brand")]
        public string FastChargerBrand { get; set; }

        [JsonPropertyName("fast_charger_present")]
        public bool FastChargerPresent { get; set; }

        [JsonPropertyName("fast_charger_type")]
        public string FastChargerType { get; set; }

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

        [JsonPropertyName("minutes_to_full_charge")]
        public int MinutesToFullCharge { get; set; }

        [JsonPropertyName("not_enough_power_to_heat")]
        public bool? NotEnoughPowerToHeat { get; set; }

        [JsonPropertyName("scheduled_charging_pending")]
        public bool ScheduledChargingPending { get; set; }

        [JsonPropertyName("scheduled_charging_start_time")]
        public DateTime? ScheduledChargingStartTime { get; set; }

        [JsonPropertyName("time_to_full_charge")]
        public double TimeToFullCharge { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("trip_charging")]
        public bool TripCharging { get; set; }

        [JsonPropertyName("usable_battery_level")]
        public int UsableBatteryLevel { get; set; }

        [JsonPropertyName("user_charge_enable_request")]
        public string UserChargeEnableRequest { get; set; }

        public override string ToString()
        {
            return $"Battery Level: {BatteryLevel}, Battery Range: {BatteryRange}, Estimated Battery Range: {EstBatteryRange}";
        }
    }
}
