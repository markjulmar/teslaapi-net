using System;
using System.Threading.Tasks;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// Represents a single Tesla vehicle tied to a Tesla account.
    /// This is the entrypoint for all of the status and command APIs.
    /// </summary>
    public class Vehicle : VehicleInfo
    {
        private TeslaClient teslaClient;

        /// <summary>
        /// Attach this vehicle to the client API
        /// </summary>
        /// <param name="client"></param>
        internal void SetClient(TeslaClient client)
        {
            teslaClient = client;
        }
        
        /// <summary>
        /// Wakes up the car from a sleeping state.
        /// The API will return a response immediately, however it could take several seconds before the car
        /// is actually online and ready to receive other commands. You can call this API with a 30-second delay
        /// until it returns 'true'.
        /// </summary>
        /// <returns>True if the car is online</returns>
        public async Task<bool> WakeupAsync()
        {
            var result = await teslaClient.PostOneAsync<Vehicle>($"{Id}/wake_up");
            
            // Copy over the fields that can change.
            Id = result.Id;
            VehicleId = result.VehicleId;
            State = result.State;
            DisplayName = result.DisplayName;
            InService = result.InService;
            ApiVersion = result.ApiVersion;
            CalendarEnabled = result.CalendarEnabled;

            return string.Compare(State, "online", StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Information on the state of charge in the battery and its various settings.
        /// </summary>
        /// <returns>Charge state object</returns>
        public Task<ChargeState> GetChargeStateAsync() => teslaClient.GetOneAsync<ChargeState>($"{Id}/data_request/charge_state");

        /// <summary>
        /// Information on the climate settings.
        /// </summary>
        /// <returns>Climate state object</returns>
        public Task<ClimateState> GetClimateStateAsync() => teslaClient.GetOneAsync<ClimateState>($"{Id}/data_request/climate_state");

        /// <summary>
        /// Returns the driving and position state of the vehicle.
        /// </summary>
        /// <returns>Drive status object</returns>
        public Task<DriveState> GetDriveStateAsync() => teslaClient.GetOneAsync<DriveState>($"{Id}/data_request/drive_state");

        /// <summary>
        /// True if mobile access setting is enabled on the car.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsMobileAccessEnabledAsync() => teslaClient.GetOneAsync<bool>($"{Id}/mobile_enabled");

        /// <summary>
        /// Retrieve the user settings related to the touchscreen interface such as driving units and locale.
        /// </summary>
        public Task<GuiSettings> GetGuiSettingsAsync() => teslaClient.GetOneAsync<GuiSettings>($"{Id}/data_request/gui_settings");

        /// <summary>
        /// Returns the vehicle's physical state, such as which doors are open.
        /// </summary>
        public Task<VehicleState> GetVehicleStateAsync() => teslaClient.GetOneAsync<VehicleState>($"{Id}/data_request/vehicle_state");

        /// <summary>
        /// Returns the vehicle's configuration information including model, color, badging and wheels.
        /// </summary>
        public Task<VehicleConfiguration> GetVehicleConfigurationAsync() => teslaClient.GetOneAsync<VehicleConfiguration>($"{Id}/data_request/vehicle_config");

        /// <summary>
        /// Returns a list of nearby Tesla-operated charging stations. (Requires car software version 2018.48 or higher.)
        /// </summary>
        public Task<ChargingStations> GetNearbyChargingStations() => teslaClient.GetOneAsync<ChargingStations>($"{Id}/nearby_charging_sites");

        /// <summary>
        /// A rollup of all the data_request endpoints plus vehicle configuration.
        /// </summary>
        public Task<VehicleDataRollup> GetAllVehicleDataAsync() => teslaClient.GetOneAsync<VehicleDataRollup>($"{Id}/vehicle_data");

        /// <summary>
        /// Honk the horn.
        /// </summary>
        /// <returns>True if success.</returns>
        public Task<bool> HonkHorn() => teslaClient.PostCommandAsync($"{Id}/command/honk_horn");

        /// <summary>
        /// Flash the headlights.
        /// </summary>
        /// <returns>True if success.</returns>
        public Task<bool> FlashLights() => teslaClient.PostCommandAsync($"{Id}/command/flash_lights");

        /// <summary>
        /// Attempt to start the car and allow a person without a key to drive the car.
        /// Once this returns, the person has 2-minutes to get into the car and drive it.
        /// </summary>
        /// <param name="accountPassword">The password for the authenticated tesla.com account.</param>
        /// <returns>True if success.</returns>
        public Task<bool> RemoteStart(string accountPassword) => 
            teslaClient.PostCommandAsync($"{Id}/command/remote_start_drive?password={accountPassword}");

        /// <summary>
        /// Lock the doors
        /// </summary>
        /// <returns>True if success.</returns>
        public Task<bool> LockDoors() => teslaClient.PostCommandAsync($"{Id}/command/door_lock");

        /// <summary>
        /// Unlock the doors.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> UnlockDoors() => teslaClient.PostCommandAsync($"{Id}/command/door_unlock");

        /// <summary>
        /// Opens or closes the primary Homelink device.
        /// The provided location must be in proximity of stored location of the Homelink device.
        /// </summary>
        /// <param name="latitude">Current latitude.</param>
        /// <param name="longitude">Current longitude.</param>
        /// <returns>True on success.</returns>
        public Task<bool> TriggerHomelink(double latitude, double longitude) => 
            teslaClient.PostCommandAsync($"{Id}/command/trigger_homelink?lat={latitude}&lon={longitude}");

        /// <summary>
        /// Sets the maximum speed allowed when Speed Limit Mode is active.
        /// </summary>
        /// <param name="mph">The speed limit in MPH. Must be between 50-90.</param>
        /// <returns>True on success.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Task<bool> SetSpeedLimit(int mph)
        {
            if (mph < 50 || mph > 90)
                throw new ArgumentException("Speed limit must be between 50 and 90.", nameof(mph));
            return teslaClient.PostCommandAsync($"{Id}/command/speed_limit_set_limit?limit_mph={mph}");
        }

        /// <summary>
        /// Set vehicle valet mode on or off with a PIN to disable it from within the car.
        /// Reuses last PIN from previous valet session. Valet Mode limits the car's top speed to 70MPH
        /// and 80kW of acceleration power. It also disables Homelink, Bluetooth and Wifi settings,
        /// and the ability to disable mobile access to the car. It also hides your favorites, home,
        /// and work locations in navigation.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> ActivateValetMode() => teslaClient.PostCommandAsync($"{Id}/command/set_valet_mode?on=true");
        
        /// <summary>
        /// Deactivates Sentry Mode. If a PIN code is required, it must be supplied.
        /// </summary>
        /// <param name="pinCode">Optional 4-digit PIN to deactivate valet mode</param>
        /// <returns>True on success.</returns>
        public Task<bool> DeactivateValetMode(string pinCode = null)
        {
            if (pinCode != null && (pinCode.Length != 4 || !int.TryParse(pinCode, out _)))
                throw new ArgumentException("PIN must be 4 digits.");
            
            string url = $"{Id}/command/set_valet_mode?on=false";
            if (string.IsNullOrEmpty(pinCode)) 
                url += $"&password={pinCode}";

            return teslaClient.PostCommandAsync(url);
        }

        /// <summary>
        /// Clears the currently set PIN for Valet Mode when deactivated. A new PIN will be required when
        /// activating from the car screen.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> ResetValetModePIN() => teslaClient.PostCommandAsync($"{Id}/command/reset_valet_pin");

        /// <summary>
        /// Turn sentry mode on.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> ActivateSentryMode() => teslaClient.PostCommandAsync($"{Id}/command/set_sentry_mode?on=true");
        
        /// <summary>
        /// Turn sentry mode off.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> DeactivateSentryMode() => teslaClient.PostCommandAsync($"{Id}/command/set_sentry_mode?on=false");

        /// <summary>
        /// Opens the trunk
        /// </summary>
        /// <returns>True on success</returns>
        public Task<bool> OpenTrunk() => teslaClient.PostCommandAsync($"{Id}/command/actuate_trunk?which_trunk=rear");

        /// <summary>
        /// Opens the front trunk (frunk)
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> OpenFrunk() => teslaClient.PostCommandAsync($"{Id}/command/actuate_trunk?which_trunk=front");

        /// <summary>
        /// Vents the windows.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> VentWindows() => teslaClient.PostCommandAsync($"{Id}/command/window_control?command=vent&lat=0&lon=0");
        
        /// <summary>
        /// Closes the windows. Latitude and longitude values must be near the current location of the car for close operation to succeed.
        /// </summary>
        /// <param name="latitude">Current latitude.</param>
        /// <param name="longitude">Current longitude.</param>
        /// <returns>True on success.</returns>
        public Task<bool> CloseWindows(double latitude, double longitude) => 
            teslaClient.PostCommandAsync($"{Id}/command/window_control?command=close&lat={latitude}&lon={longitude}");

        /// <summary>
        /// Vents the panoramic sunroof on the Model S.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> VentSunroof() => teslaClient.PostCommandAsync($"{Id}/command/sun_roof_control?state=vent");
        
        /// <summary>
        /// Closes the panoramic sunroof on the Model S.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> CloseSunroof() => teslaClient.PostCommandAsync($"{Id}/command/sun_roof_control?state=close");

        /// <summary>
        /// Opens/Unlocks the charging port.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> OpenChargingDoor() => teslaClient.PostCommandAsync($"{Id}/command/charge_port_door_open");

        /// <summary>
        /// Closes the charging port on cars with a motorized charging port.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> CloseChargingDoor() => teslaClient.PostCommandAsync($"{Id}/command/charge_port_door_close");

        /// <summary>
        /// If the car is plugged in but not currently charging, this will start it charging.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> StartCharging() => teslaClient.PostCommandAsync($"{Id}/command/charge_start");

        /// <summary>
        /// If the car is currently charging, this will stop it.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> StopCharging() => teslaClient.PostCommandAsync($"{Id}/command/charge_stop");

        /// <summary>
        /// If the car is currently charging, this will stop it.
        /// </summary>
        /// <param name="percentage">Percentage to set to. If omitted, will use 'standard' limit.</param>
        /// <returns>True on success.</returns>
        public Task<bool> SetChargeLimit(int? percentage = null)
        {
            if (!percentage.HasValue || percentage.Value < 0)
                return teslaClient.PostCommandAsync($"{Id}/command/charge_standard");

            // Custom or Max range
            return teslaClient.PostCommandAsync(percentage.Value >= 100 
                ? $"{Id}/command/charge_max_range" 
                : $"{Id}/command/set_charge_limit?percent={percentage}");
        }

        /// <summary>
        /// Start the climate control (HVAC) system. Will cool or heat automatically, depending on set temperature.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> StartClimateControl() =>
            teslaClient.PostCommandAsync($"{Id}/command/auto_conditioning_start");
        
        /// <summary>
        /// Stop the climate control (HVAC) system.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> StopClimateControl() =>
            teslaClient.PostCommandAsync($"{Id}/command/auto_conditioning_stop");

        /// <summary>
        /// Sets the target temperature for the climate control (HVAC) system.
        /// </summary>
        /// <param name="celsiusTemp">The desired temperature in celsius.</param>
        /// <returns>True on success.</returns>
        public Task<bool> SetClimateControlTemperature(double celsiusTemp) =>
            teslaClient.PostCommandAsync($"{Id}/command/set_temps?driver_temp={celsiusTemp}&passenger_temp={celsiusTemp}");
        
        /// <summary>
        /// Turn on the max defrost setting.
        /// </summary>
        /// <returns>True on success</returns>
        public Task<bool> StartClimateControlDefrost() => teslaClient.PostCommandAsync($"{Id}/command/set_preconditioning_max?on=true");

        /// <summary>
        /// Turn off the max defrost setting. Switches climate control back to default setting.
        /// </summary>
        /// <returns>True on success</returns>
        public Task<bool> StopClimateControlDefrost() => teslaClient.PostCommandAsync($"{Id}/command/set_preconditioning_max?on=false");

        /// <summary>
        /// Sets the specified seat's heater level.
        /// </summary>
        /// <param name="seat">The desired seat to heat.</param>
        /// <param name="value">The desired level for the heater.</param>
        /// <returns>True on success.</returns>
        public Task<bool> SetSeatHeater(Seat seat, SeatHeater value) =>
            teslaClient.PostCommandAsync($"{Id}/command/remote_seat_heater_request?heater={(int)seat}&level={(int)value}");

        /// <summary>
        /// Turn steering wheel heater on or off.
        /// </summary>
        /// <param name="onOff"></param>
        /// <returns>True on success.</returns>
        public Task<bool> SetSteeringWheelHeater(bool onOff) =>
            teslaClient.PostCommandAsync(
                $"{Id}/command/remote_steering_wheel_heater_request?on={onOff.ToString().ToLower()}");

        /// <summary>
        /// Toggles the media between playing and paused. For the radio, this mutes or unmutes the audio.
        /// The car must be on.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> MediaTogglePlayPause() => teslaClient.PostCommandAsync($"{Id}/command/media_toggle_playback");

        /// <summary>
        /// Skips to the next track in the current playlist, or to the next station.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> MediaNextTrack() => teslaClient.PostCommandAsync($"{Id}/command/media_next_track");

        /// <summary>
        /// Skips to the previous track in the current playlist, or to the next station. Does nothing for streaming stations.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> MediaPreviousTrack() => teslaClient.PostCommandAsync($"{Id}/command/media_prev_track");

        /// <summary>
        /// Skips to the next saved favorite in the media system.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> MediaNextFavorite() => teslaClient.PostCommandAsync($"{Id}/command/media_next_fav");
        
        /// <summary>
        /// Skips to the previous saved favorite in the media system.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> MediaPreviousFavorite() => teslaClient.PostCommandAsync($"{Id}/command/media_prev_fav");
        
        /// <summary>
        /// Turns up the volume of the media system.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> MediaVolumeUp() => teslaClient.PostCommandAsync($"{Id}/command/media_volume_up");
        
        /// <summary>
        /// Turns down the volume of the media system.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> MediaVolumeDown() => teslaClient.PostCommandAsync($"{Id}/command/media_volume_down");
        
        /// <summary>
        /// Schedules a software update to be installed, if one is available.
        /// </summary>
        /// <param name="delayInSeconds">How many seconds in the future to schedule the update. Set to 0 for immediate install.</param>
        /// <returns>True on success.</returns>
        public Task<bool> ScheduleSoftwareUpdate(int delayInSeconds = 0) => teslaClient.PostCommandAsync($"{Id}/command/schedule_software_update");
        
        /// <summary>
        /// Cancels a software update, if one is scheduled and has not yet started.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> CancelSoftwareUpdate() => teslaClient.PostCommandAsync($"{Id}/command/cancel_software_update");

        /// <summary>
        /// Set upcoming calendar entries.
        /// </summary>
        /// <returns>True on success.</returns>
        public Task<bool> SetUpcomingCalendarEntries() => teslaClient.PostCommandAsync($"{Id}/command/upcoming_calendar_entries");

        /// <summary>
        /// Sends a location for the car to start navigation.
        /// </summary>
        /// <param name="address">Address to navigate to</param>
        /// <param name="locale">Locale of address - if not supplied, current UI culture is used.</param>
        /// <returns>True on success.</returns>
        public Task<bool> ShareAddressForNavigation(string address, string locale = null) =>
            teslaClient.PostCommandAsync($"{Id}/command/share", new ShareRequest(address, locale));

        /// <summary>
        /// Play a video in theatre mode.
        /// </summary>
        /// <param name="uri">Web address for video to play</param>
        /// <returns>True on success.</returns>
        public Task<bool> PlayFullscreenVideo(Uri uri) =>
            teslaClient.PostCommandAsync($"{Id}/command/share", new ShareRequest(uri.AbsoluteUri));
    }
}
