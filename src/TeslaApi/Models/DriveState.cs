using System;
using System.Text.Json.Serialization;
using Julmar.TeslaApi.Internal;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// This JSON object is returned from /api/1/vehicles/{id}/data_request/drive_state
    /// </summary>
    public sealed class DriveState
    {
        /// <summary>
        /// Last time GPS was recorded.
        /// </summary>
        [JsonPropertyName("gps_as_of"), JsonConverter(typeof(TimestampToDateTimeConverter))]
        public DateTime? GpsAsOf { get; set; }

        /// <summary>
        /// Current directional heading
        /// </summary>
        [JsonPropertyName("heading")]
        public int Heading { get; set; }

        /// <summary>
        /// GPS location
        /// </summary>
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// GPS location
        /// </summary>
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// GPS location
        /// </summary>
        [JsonPropertyName("native_latitude")]
        public double NativeLatitude { get; set; }

        /// <summary>
        /// True if the native long/lat are valid. 
        /// </summary>
        [JsonPropertyName("native_location_supported"), JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool NativeLocationSupported { get; set; }

        /// <summary>
        /// GPS location
        /// </summary>
        [JsonPropertyName("native_longitude")]
        public double NativeLongitude { get; set; }

        /// <summary>
        /// How the native GPS is represented
        /// wgs = World Geodetic System
        /// </summary>
        [JsonPropertyName("native_type")]
        public string NativeType { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("power")]
        public int Power { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("shift_state")]
        public string ShiftState { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("speed")]
        public double? Speed { get; set; }

        /// <summary>
        /// Timestamp for this object
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        public override string ToString()
        {
            return $"Power: {Power}, GPS: (Lat={Latitude}, Long={Longitude}, Heading={Heading}) as of {GpsAsOf}, Speed: {Speed}";
        }
    }
}
