using System;
using System.Text.Json.Serialization;
using Julmar.TeslaApi.Internal;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// Media status
    /// </summary>
    public sealed class MediaState
    {
        /// <summary>
        /// True if media can be controlled remotely.
        /// </summary>
        [JsonPropertyName("remote_control_enabled")]
        public bool RemoteControlEnabled { get; set; }
    }

    /// <summary>
    /// Software version and update information.
    /// </summary>
    public sealed class SoftwareUpdate
    {
        /// <summary>
        /// 0-100% of next version downloaded.
        /// </summary>
        [JsonPropertyName("download_perc")]
        public int DownloadPercentage { get; set; }

        /// <summary>
        /// How long it should take to download in seconds.
        /// </summary>
        [JsonPropertyName("expected_duration_sec")]
        public int ExpectedDurationSeconds { get; set; }

        /// <summary>
        /// Installation percentage (0-100)
        /// </summary>
        [JsonPropertyName("install_perc")]
        public int InstallPercentage { get; set; }

        /// <summary>
        /// Current software update status
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Currently installed version.
        /// </summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }
    }

    /// <summary>
    /// Speed limitations applied
    /// </summary>
    public sealed class SpeedLimitMode
    {
        /// <summary>
        /// True if speed limiting is turned on.
        /// </summary>
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        /// <summary>
        /// MPH limit.
        /// </summary>
        [JsonPropertyName("current_limit_mph")]
        public double CurrentLimitMph { get; set; }

        /// <summary>
        /// Maximum limit allowed (90)
        /// </summary>
        [JsonPropertyName("max_limit_mph")]
        public int MaxLimitMph { get; set; }

        /// <summary>
        /// Minimum limit allowed (50)
        /// </summary>
        [JsonPropertyName("min_limit_mph")]
        public int MinLimitMph { get; set; }

        /// <summary>
        /// True if a PIN code has been set and must be supplied to turn off speed limiting.
        /// </summary>
        [JsonPropertyName("pin_code_set")]
        public bool PinCodeSet { get; set; }
    }

    /// <summary>
    /// Current display state
    /// </summary>
    public enum CenterDisplayState
    {
        /// <summary>
        /// Display is off.
        /// </summary>
        Off = 0,
        /// <summary>
        /// Display is on standby.
        /// </summary>
        OnStandbyOrCampMode = 2,
        /// <summary>
        /// Display is on charging screen.
        /// </summary>
        OnChargingScreen = 3,
        /// <summary>
        /// Display is on main screen.
        /// </summary>
        On = 4,
        /// <summary>
        /// Display is on large (clock) charging screen.
        /// </summary>
        OnBigChargingScreen = 5,
        /// <summary>
        /// Ready to unlock (PIN)
        /// </summary>
        OnReadyToUnlock = 6,
        /// <summary>
        /// Display is in sentry mode.
        /// </summary>
        SentryMode = 7,
        /// <summary>
        /// Display is in dog mode.
        /// </summary>
        DogMode = 8,
        /// <summary>
        /// Display is in media mode.
        /// </summary>
        Media = 9
    }

    /// <summary>
    /// Main JSON object returned from /api/1/vehicles/{id}/data_request/vehicle_state
    /// </summary>
    public sealed class VehicleState
    {
        /// <summary>
        /// Current API version
        /// </summary>
        [JsonPropertyName("api_version")]
        public int ApiVersion { get; set; }

        /// <summary>
        /// Autopark state
        /// </summary>
        [JsonPropertyName("autopark_state_v2")]
        public string AutoparkStateV2 { get; set; }

        /// <summary>
        /// Autopark style
        /// </summary>
        [JsonPropertyName("autopark_style")]
        public string AutoparkStyle { get; set; }

        /// <summary>
        /// True if calendar can be read from mobile device.
        /// </summary>
        [JsonPropertyName("calendar_supported")]
        public bool CalendarSupported { get; set; }

        /// <summary>
        /// Current screen status
        /// </summary>
        [JsonPropertyName("center_display_state")]
        public CenterDisplayState CenterDisplayState { get; set; }

        /// <summary>
        /// True if driver front door is open
        /// </summary>
        [JsonPropertyName("df"), JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool FrontDriverDoorOpen { get; set; }

        /// <summary>
        /// True if driver front window is open/vented
        /// </summary>
        [JsonPropertyName("fd_window"), JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool FrontDriverWindowOpen { get; set; }

        /// <summary>
        /// True if passenger front window is open/vented
        /// </summary>
        [JsonPropertyName("fp_window"), JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool FrontPassengerWindowOpen { get; set; }

        /// <summary>
        /// True if driver rear door is open
        /// </summary>
        [JsonPropertyName("dr"), JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool RearDriverDoorOpen { get; set; }

        /// <summary>
        /// True if front trunk is open
        /// </summary>
        [JsonPropertyName("ft"), JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool FrontTrunkOpen { get; set; }

        /// <summary>
        /// # of configured homelink devices
        /// </summary>
        [JsonPropertyName("homelink_device_count")]
        public int HomelinkDeviceCount { get; set; }

        /// <summary>
        /// True if the primary homelink device is nearby
        /// </summary>
        [JsonPropertyName("homelink_nearby")]
        public bool HomelinkNearby { get; set; }

        /// <summary>
        /// True if a person is in the car
        /// </summary>
        [JsonPropertyName("is_user_present")]
        public bool IsUserPresent { get; set; }

        /// <summary>
        /// Last autopark error
        /// </summary>
        [JsonPropertyName("last_autopark_error")]
        public string LastAutoparkError { get; set; }

        /// <summary>
        /// True if the car is locked
        /// </summary>
        [JsonPropertyName("locked")]
        public bool Locked { get; set; }

        /// <summary>
        /// Current media status
        /// </summary>
        [JsonPropertyName("media_state")]
        public MediaState MediaState { get; set; }

        /// <summary>
        /// True if notifications are supported
        /// </summary>
        [JsonPropertyName("notifications_supported")]
        public bool NotificationsSupported { get; set; }

        /// <summary>
        /// Current odometer reading
        /// </summary>
        [JsonPropertyName("odometer")]
        public double Odometer { get; set; }

        /// <summary>
        /// TBD
        /// </summary>
        [JsonPropertyName("parsed_calendar_supported")]
        public bool ParsedCalendarSupported { get; set; }

        /// <summary>
        /// True if passenger front door is open
        /// </summary>
        [JsonPropertyName("pf"), JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool FrontPassengerDoorOpen { get; set; }

        /// <summary>
        /// True if passenger rear door is open
        /// </summary>
        [JsonPropertyName("pr"), JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool RearPassengerDoorOpen { get; set; }

        /// <summary>
        /// True if rear passenger window is open/vented
        /// </summary>
        [JsonPropertyName("rp_window"), JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool RearPassengerWindowOpen { get; set; }

        /// <summary>
        /// True if driver rear window is open/vented
        /// </summary>
        [JsonPropertyName("rd_window"), JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool RearDriverWindowOpen { get; set; }

        /// <summary>
        /// True if remote start is active (2-min timer)
        /// </summary>
        [JsonPropertyName("remote_start")]
        public bool RemoteStart { get; set; }

        /// <summary>
        /// True if remote start is enabled.
        /// </summary>
        [JsonPropertyName("remote_start_enabled")]
        public bool RemoteStartEnabled { get; set; }

        /// <summary>
        /// True if remote start is supported.
        /// </summary>
        [JsonPropertyName("remote_start_supported")]
        public bool RemoteStartSupported { get; set; }

        /// <summary>
        /// True if rear trunk is open
        /// </summary>
        [JsonPropertyName("rt"), JsonConverter(typeof(IntToBoolJsonConverter))]
        public bool RearTrunkOpen { get; set; }

        /// <summary>
        /// True if car is in sentry mode
        /// </summary>
        [JsonPropertyName("sentry_mode")]
        public bool SentryMode { get; set; }

        /// <summary>
        /// True if sentry mode is available.
        /// </summary>
        [JsonPropertyName("sentry_mode_available")]
        public bool SentryModeAvailable { get; set; }

        /// <summary>
        /// True if car can be summoned.
        /// </summary>
        [JsonPropertyName("smart_summon_available")]
        public bool SmartSummonAvailable { get; set; }

        /// <summary>
        /// Software update available
        /// </summary>
        [JsonPropertyName("software_update")]
        public SoftwareUpdate SoftwareUpdate { get; set; }

        /// <summary>
        /// Speed limiter information.
        /// </summary>
        [JsonPropertyName("speed_limit_mode")]
        public SpeedLimitMode SpeedLimitMode { get; set; }

        /// <summary>
        /// True if summon standby mode enabled
        /// </summary>
        [JsonPropertyName("summon_standby_mode_enabled")]
        public bool SummonStandbyModeEnabled { get; set; }

        /// <summary>
        /// % Sun roof is open (model S only)
        /// </summary>
        [JsonPropertyName("sun_roof_percent_open")]
        public int SunRoofPercentOpen { get; set; }

        /// <summary>
        /// Current sun roof status (model S only)
        /// </summary>
        [JsonPropertyName("sun_roof_state")]
        public string SunRoofState { get; set; }

        /// <summary>
        /// Object timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        /// <summary>
        /// True if valet mode active
        /// </summary>
        [JsonPropertyName("valet_mode")]
        public bool ValetMode { get; set; }

        /// <summary>
        /// True if valet PIN is required
        /// </summary>
        [JsonPropertyName("valet_pin_needed")]
        public bool ValetPinNeeded { get; set; }

        /// <summary>
        /// Vehicle name
        /// </summary>
        [JsonPropertyName("vehicle_name")]
        public string VehicleName { get; set; }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
            => $"{VehicleName} - Frunk: {FrontTrunkOpen}, Trunk: {RearTrunkOpen}, Odometer: {Odometer}, Console: {CenterDisplayState}, SentryAvail: {SentryModeAvailable}, SentryMode: {SentryMode}";
    }
}
