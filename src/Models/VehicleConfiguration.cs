using System;
using System.Text.Json.Serialization;
using Julmar.TeslaApi.Internal;

namespace Julmar.TeslaApi
{
    public sealed class VehicleConfiguration
    {
        /// <summary>
        /// True if the car can receive map requests through /share
        /// </summary>
        [JsonPropertyName("can_accept_navigation_requests")]
        public bool CanAcceptNavigationRequests { get; set; }

        /// <summary>
        /// True if the trunk can open/close automatically
        /// </summary>
        [JsonPropertyName("can_actuate_trunks")]
        public bool CanActuateTrunks { get; set; }

        /// <summary>
        /// Extra info about car.
        /// </summary>
        [JsonPropertyName("car_special_type")]
        public string CarSpecialType { get; set; }

        /// <summary>
        /// Car type
        /// model3, etc.
        /// </summary>
        [JsonPropertyName("car_type")]
        public string CarType { get; set; }

        /// <summary>
        /// Charge port type
        /// "US"
        /// </summary>
        [JsonPropertyName("charge_port_type")]
        public string ChargePortType { get; set; }

        /// <summary>
        /// True if the vehicle has ECE regulations
        /// </summary>
        [JsonPropertyName("ece_restrictions")]
        public bool EceRestrictions { get; set; }

        /// <summary>
        /// True if this is an EU vehicle.
        /// </summary>
        [JsonPropertyName("eu_vehicle")]
        public bool EuVehicle { get; set; }

        /// <summary>
        /// Exterior color
        /// </summary>
        [JsonPropertyName("exterior_color")]
        public string ExteriorColor { get; set; }

        /// <summary>
        /// True if the car has air suspension.
        /// </summary>
        [JsonPropertyName("has_air_suspension")]
        public bool HasAirSuspension { get; set; }

        /// <summary>
        /// True if the car has ludicrous mode
        /// </summary>
        [JsonPropertyName("has_ludicrous_mode")]
        public bool HasLudicrousMode { get; set; }

        /// <summary>
        /// True if the charge port is motorized.
        /// </summary>
        [JsonPropertyName("motorized_charge_port")]
        public bool MotorizedChargePort { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("plg")]
        public bool Plg { get; set; }

        /// <summary>
        /// True if the car has rear seat heaters
        /// </summary>
        [JsonPropertyName("rear_seat_heaters"), JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool RearSeatHeaters { get; set; }

        /// <summary>
        /// Rear seat type
        /// </summary>
        [JsonPropertyName("rear_seat_type")]
        public int? RearSeatType { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("rhd")]
        public bool Rhd { get; set; }

        /// <summary>
        /// Roof color
        /// </summary>
        [JsonPropertyName("roof_color")]
        public string RoofColor { get; set; }

        /// <summary>
        /// Seat type
        /// </summary>
        [JsonPropertyName("seat_type")]
        public int? SeatType { get; set; }

        /// <summary>
        /// Spoiler type
        /// </summary>
        [JsonPropertyName("spoiler_type")]
        public string SpoilerType { get; set; }

        /// <summary>
        /// Type of sun roof (null for none)
        /// </summary>
        [JsonPropertyName("sun_roof_installed")]
        public int? SunRoofInstalled { get; set; }

        /// <summary>
        /// Type of third row seats, "None" for none.
        /// </summary>
        [JsonPropertyName("third_row_seats")]
        public string ThirdRowSeats { get; set; }

        /// <summary>
        /// Timestamp of object.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("use_range_badging")]
        public bool UseRangeBadging { get; set; }

        /// <summary>
        /// Type of wheels
        /// </summary>
        [JsonPropertyName("wheel_type")]
        public string WheelType { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() 
            => $"CarType: {CarType}, ExteriorColor: {ExteriorColor}, RoofColor: {RoofColor}, WheelType: {WheelType}";
    }
}
