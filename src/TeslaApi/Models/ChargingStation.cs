using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// Location (lat/long)
    /// </summary>
    public sealed class Location
    {
        /// <summary>
        /// Latitude of location
        /// </summary>
        [JsonPropertyName("lat")]
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude of location
        /// </summary>
        [JsonPropertyName("long")]
        public double Longitude { get; set; }
    }

    /// <summary>
    /// JSON object representing a single destination charger.
    /// </summary>
    public abstract class ChargerLocation
    {
        /// <summary>
        /// Map location of charger.
        /// </summary>
        [JsonPropertyName("location")]
        public Location Location { get; set; }

        /// <summary>
        /// User friendly name of charger.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// Type of charger
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Distance from vehicle in miles.
        /// </summary>
        [JsonPropertyName("distance_miles")]
        public double DistanceMiles { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() 
            => $"{Name} {Type} - {Location}: {DistanceMiles} mi away";
    }

    /// <summary>
    /// JSON object representing a single destination charger.
    /// </summary>
    public sealed class DestinationCharger : ChargerLocation
    {
    }

    /// <summary>
    /// JSON object representing a Tesla Supercharger
    /// </summary>
    public sealed class Supercharger : ChargerLocation
    {
        /// <summary>
        /// Number of available stalls
        /// </summary>
        [JsonPropertyName("available_stalls")]
        public int AvailableStalls { get; set; }

        /// <summary>
        /// Total number of stalls
        /// </summary>
        [JsonPropertyName("total_stalls")]
        public int TotalStalls { get; set; }

        /// <summary>
        /// True if the site is closed.
        /// </summary>
        [JsonPropertyName("site_closed")]
        public bool SiteClosed { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return SiteClosed ? $"{Name} {Type} site closed."
                : $"{Name} {Type} - {DistanceMiles} mi away, {AvailableStalls}/{TotalStalls}";
        }
    }

    /// <summary>
    /// Returning JSON object for /api/1/vehicles/{id}/nearby_charging_sites
    /// </summary>
    public sealed class ChargingStations
    {
        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("congestion_sync_time_utc_secs")]
        public int CongestionSyncTimeUtcSecs { get; set; }

        /// <summary>
        /// List of all close destination chargers
        /// </summary>
        [JsonPropertyName("destination_charging")]
        public List<DestinationCharger> DestinationChargers { get; set; }

        /// <summary>
        /// List of all close Tesla superchargers
        /// </summary>
        [JsonPropertyName("superchargers")]
        public List<Supercharger> Superchargers { get; set; }

        /// <summary>
        /// Timestamp for this object.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() 
            => $"{DestinationChargers.Count} destination chargers, {Superchargers.Count} superchargers";
    }
}
