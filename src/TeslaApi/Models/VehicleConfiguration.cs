using System;
using System.Text.Json.Serialization;

namespace TeslaApi.Models
{
    public class VehicleConfiguration
    {
        [JsonPropertyName("can_accept_navigation_requests")]
        public bool CanAcceptNavigationRequests { get; set; }

        [JsonPropertyName("can_actuate_trunks")]
        public bool CanActuateTrunks { get; set; }

        [JsonPropertyName("car_special_type")]
        public string CarSpecialType { get; set; }

        [JsonPropertyName("car_type")]
        public string CarType { get; set; }

        [JsonPropertyName("charge_port_type")]
        public string ChargePortType { get; set; }

        [JsonPropertyName("ece_restrictions")]
        public bool EceRestrictions { get; set; }

        [JsonPropertyName("eu_vehicle")]
        public bool EuVehicle { get; set; }

        [JsonPropertyName("exterior_color")]
        public string ExteriorColor { get; set; }

        [JsonPropertyName("has_air_suspension")]
        public bool HasAirSuspension { get; set; }

        [JsonPropertyName("has_ludicrous_mode")]
        public bool HasLudicrousMode { get; set; }

        [JsonPropertyName("motorized_charge_port")]
        public bool MotorizedChargePort { get; set; }

        [JsonPropertyName("plg")]
        public bool Plg { get; set; }

        [JsonPropertyName("rear_seat_heaters")]
        public int RearSeatHeaters { get; set; }

        [JsonPropertyName("rear_seat_type")]
        public int? RearSeatType { get; set; }

        [JsonPropertyName("rhd")]
        public bool Rhd { get; set; }

        [JsonPropertyName("roof_color")]
        public string RoofColor { get; set; }

        [JsonPropertyName("seat_type")]
        public int? SeatType { get; set; }

        [JsonPropertyName("spoiler_type")]
        public string SpoilerType { get; set; }

        [JsonPropertyName("sun_roof_installed")]
        public int? SunRoofInstalled { get; set; }

        [JsonPropertyName("third_row_seats")]
        public string ThirdRowSeats { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("trim_badging")]
        public string TrimBadging { get; set; }

        [JsonPropertyName("use_range_badging")]
        public bool UseRangeBadging { get; set; }

        [JsonPropertyName("wheel_type")]
        public string WheelType { get; set; }

        public override string ToString()
        {
            return $"CarType: {CarType}, ExteriorColor: {ExteriorColor}, TrimBadging: {TrimBadging}, WheelType: {WheelType}";
        }
    }
}
