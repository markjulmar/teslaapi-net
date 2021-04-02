using System.Text.Json.Serialization;

namespace TeslaApi
{
    public class DriveState
    {
        [JsonPropertyName("gps_as_of")]
        public int GpsAsOf { get; set; }

        [JsonPropertyName("heading")]
        public int Heading { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("native_latitude")]
        public double NativeLatitude { get; set; }

        [JsonPropertyName("native_location_supported")]
        public int NativeLocationSupported { get; set; }

        [JsonPropertyName("native_longitude")]
        public double NativeLongitude { get; set; }

        [JsonPropertyName("native_type")]
        public string NativeType { get; set; }

        [JsonPropertyName("power")]
        public int Power { get; set; }

        [JsonPropertyName("shift_state")]
        public string ShiftState { get; set; }

        [JsonPropertyName("speed")]
        public string Speed { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        public override string ToString()
        {
            return $"Power: {Power}, Latitude: {Latitude}, Longitude: {Longitude}, Speed: {Speed}";
        }
    }
}
