using System;
using System.Text.Json.Serialization;

namespace Julmar.TeslaApi
{
    public sealed class MediaState
    {
        [JsonPropertyName("remote_control_enabled")]
        public bool RemoteControlEnabled { get; set; }
    }

    public sealed class SoftwareUpdate
    {
        [JsonPropertyName("download_perc")]
        public int DownloadPercentage { get; set; }

        [JsonPropertyName("expected_duration_sec")]
        public int ExpectedDurationSeconds { get; set; }

        [JsonPropertyName("install_perc")]
        public int InstallPercentage { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }
    }

    public sealed class SpeedLimitMode
    {
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("current_limit_mph")]
        public double CurrentLimitMph { get; set; }

        [JsonPropertyName("max_limit_mph")]
        public int MaxLimitMph { get; set; }

        [JsonPropertyName("min_limit_mph")]
        public int MinLimitMph { get; set; }

        [JsonPropertyName("pin_code_set")]
        public bool PinCodeSet { get; set; }
    }

    public enum CenterDisplayState
    {
        Off = 0,
        OnStandbyOrCampMode = 2,
        OnChargingScreen = 3,
        On = 4,
        OnBigChargingScreen = 5,
        OnReadyToUnlock = 6,
        SentryMode = 7,
        DogMode = 8,
        Media = 9
    }

    public sealed class VehicleState
    {
        [JsonPropertyName("api_version")]
        public int ApiVersion { get; set; }

        [JsonPropertyName("autopark_state_v2")]
        public string AutoparkStateV2 { get; set; }

        [JsonPropertyName("autopark_style")]
        public string AutoparkStyle { get; set; }

        [JsonPropertyName("calendar_supported")]
        public bool CalendarSupported { get; set; }

        [JsonPropertyName("car_version")]
        public string CarVersion { get; set; }

        [JsonPropertyName("center_display_state")]
        public CenterDisplayState CenterDisplayState { get; set; }

        [JsonPropertyName("df")]
        public int DriverFront { get; set; }

        [JsonPropertyName("dr")]
        public int DriverRear { get; set; }

        [JsonPropertyName("ft")]
        public bool FrontTrunk { get; set; }

        [JsonPropertyName("homelink_device_count")]
        public int HomelinkDeviceCount { get; set; }

        [JsonPropertyName("homelink_nearby")]
        public bool HomelinkNearby { get; set; }

        [JsonPropertyName("is_user_present")]
        public bool IsUserPresent { get; set; }

        [JsonPropertyName("last_autopark_error")]
        public string LastAutoparkError { get; set; }

        [JsonPropertyName("locked")]
        public bool Locked { get; set; }

        [JsonPropertyName("media_state")]
        public MediaState MediaState { get; set; }

        [JsonPropertyName("notifications_supported")]
        public bool NotificationsSupported { get; set; }

        [JsonPropertyName("odometer")]
        public double Odometer { get; set; }

        [JsonPropertyName("parsed_calendar_supported")]
        public bool ParsedCalendarSupported { get; set; }

        [JsonPropertyName("pf")]
        public int PassengerFront { get; set; }

        [JsonPropertyName("pr")]
        public int PassengerRear { get; set; }

        [JsonPropertyName("remote_start")]
        public bool RemoteStart { get; set; }

        [JsonPropertyName("remote_start_enabled")]
        public bool RemoteStartEnabled { get; set; }

        [JsonPropertyName("remote_start_supported")]
        public bool RemoteStartSupported { get; set; }

        [JsonPropertyName("rt")]
        public bool RearTrunk { get; set; }

        [JsonPropertyName("sentry_mode")]
        public bool SentryMode { get; set; }

        [JsonPropertyName("sentry_mode_available")]
        public bool SentryModeAvailable { get; set; }

        [JsonPropertyName("smart_summon_available")]
        public bool SmartSummonAvailable { get; set; }

        [JsonPropertyName("software_update")]
        public SoftwareUpdate SoftwareUpdate { get; set; }

        [JsonPropertyName("speed_limit_mode")]
        public SpeedLimitMode SpeedLimitMode { get; set; }

        [JsonPropertyName("summon_standby_mode_enabled")]
        public bool SummonStandbyModeEnabled { get; set; }

        [JsonPropertyName("sun_roof_percent_open")]
        public int SunRoofPercentOpen { get; set; }

        [JsonPropertyName("sun_roof_state")]
        public string SunRoofState { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("valet_mode")]
        public bool ValetMode { get; set; }

        [JsonPropertyName("valet_pin_needed")]
        public bool ValetPinNeeded { get; set; }

        [JsonPropertyName("vehicle_name")]
        public string VehicleName { get; set; }

        public override string ToString()
        {
            return $"{VehicleName} - Frunk: {FrontTrunk}, Trunk: {RearTrunk}, Odometer: {Odometer}, Console: {CenterDisplayState}, SentryAvail: {SentryModeAvailable}, SentryMode: {SentryMode}";
        }
    }
}
