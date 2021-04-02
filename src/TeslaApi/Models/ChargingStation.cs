using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TeslaApi.Models
{
    public class Location
    {
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }

        [JsonPropertyName("long")]
        public double Longitude { get; set; }
    }

    public class DestinationCharging
    {
        [JsonPropertyName("location")]
        public Location Location { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("distance_miles")]
        public double DistanceMiles { get; set; }

        public override string ToString()
        {
            return $"{Name} {Type} - {Location}: {DistanceMiles} mi away";
        }
    }

    public class Supercharger
    {
        [JsonPropertyName("location")]
        public Location Location { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("distance_miles")]
        public double DistanceMiles { get; set; }

        [JsonPropertyName("available_stalls")]
        public int AvailableStalls { get; set; }

        [JsonPropertyName("total_stalls")]
        public int TotalStalls { get; set; }

        [JsonPropertyName("site_closed")]
        public bool SiteClosed { get; set; }

        public override string ToString()
        {
            return SiteClosed ? $"{Name} {Type} site closed."
                : $"{Name} {Type} - {DistanceMiles} mi away, {AvailableStalls}/{TotalStalls}";
        }
    }

    public class ChargingStations
    {
        [JsonPropertyName("congestion_sync_time_utc_secs")]
        public int CongestionSyncTimeUtcSecs { get; set; }

        [JsonPropertyName("destination_charging")]
        public List<DestinationCharging> DestinationCharging { get; set; }

        [JsonPropertyName("superchargers")]
        public List<Supercharger> Superchargers { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        public override string ToString()
        {
            return $"{DestinationCharging.Count} destination chargers, {Superchargers.Count} superchargers";
        }
    }
}
