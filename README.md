# TeslaApi.NET

This is a .NET Standard library for interacting with the [undocumented Tesla REST API](https://www.teslaapi.io/). It provides an easy way for .NET Core and desktop apps to retrieve status and send commands to Tesla vehicles.

[![Build and Publish Tesla.NET](https://github.com/markjulmar/teslaapi-net/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/markjulmar/teslaapi-net/actions/workflows/dotnet.yml)

## Getting the library

You can clone this repo and build from source, or install into your .NET app with NuGet.

```console
Install-Package Julmar.TeslaApi -Version 1.0.2-prerelease
```

If you any any issues, please report them here. Even better, this project takes pull requests!

## Working with the API

Everything starts with the [TeslaClient](https://github.com/markjulmar/teslaapi-net/blob/main/src/TeslaApi/TeslaClient.cs) object. This provides the methods used to authenticate with the REST service and retrieve a vehicle object.

You can create one directly:

```csharp
var client = new TeslaClient();
```

Or, if you have an existing access token, you can create one from that:

```csharp
var client = TeslaClient.CreateFromToken("{my-access-token}");
```

Here's the full class definition:

```csharp
public class TeslaClient : IDisposable
{
    /// <summary>
    /// Constructor for the TeslaClient.
    /// </summary>
    public TeslaClient();
    
    /// <summary>
    /// Create a TeslaClient from an access token.
    /// </summary>
    /// <param name="token">Tesla owner account access token.</param>
    /// <returns>TeslaAccount object</returns>
    public static TeslaClient CreateFromToken(string token);
    
    /// <summary>
    /// Client Id used to create owner account token. This is set to a default
    /// value, but could be invalidated in the future by Tesla, in which case clients
    /// can provide a new value through this property before logging in.
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    /// Client Secret used to create owner account token. This is set to a default
    /// value, but could be invalidated in the future by Tesla, in which case clients
    /// can provide a new value through this property before logging in.
    /// </summary>
    public string ClientSecret { get; set; }
    
    /// <summary>
    /// Diagnostic logging support. Set this to a delegate function which will be used
    /// to log traffic through this TeslaClient object.
    /// </summary>
    public Action<LogLevel, string> TraceLog { get; set; }
    
    /// <summary>
    /// Function to auto-refresh token and reissue any failing call.
    /// </summary>
    public Func<string> AutoRefreshToken { get; set; }

    /// <summary>
    /// Login to the Tesla API and retrieve an owner API token.
    /// </summary>
    /// <param name="email">Email account tied to Tesla</param>
    /// <param name="password">Password for the Tesla account</param>
    /// <param name="multiFactorAuthResolver">Optional resolver for Multi-Factor auth.</param>
    /// <returns>Access token information</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<AccessToken> LoginAsync(string email, string password, Func<(string passcode, string backupPasscode)> multiFactorAuthResolver = null);

    /// <summary>
    /// Used to revoke an app token
    /// </summary>
    /// <param name="token">Token to revoke</param>
    /// <exception cref="TeslaAuthenticationException"></exception>
    public Task RevokeTokenAsync(string token);
    
    /// <summary>
    /// Method to refresh the access token from a refresh token.
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>New access token information</returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<AccessToken> RefreshLoginAsync(string refreshToken);

    /// <summary>
    /// Retrieve a list of all the vehicles tied to this account.
    /// </summary>
    /// <returns>List of vehicles.</returns>
    public async Task<IReadOnlyList<Vehicle>> GetVehiclesAsync();

    /// <summary>
    /// Return a single vehicle object based on the unique identifier.
    /// </summary>
    /// <param name="id">Id of the car</param>
    /// <returns>Vehicle object</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task<Vehicle> GetVehicleAsync(long id);

    /// <summary>
    /// Dispose the underlying connection. Note that it can be
    /// recreated by an attached vehicle.
    /// </summary>
    public void Dispose();
}
```

## Authentication

The first step in using the API is to authenticate and retrieve an access token. This is done through the `LoginAsync` method on the `TeslaClient` object. This method takes several parameters:

| Parameter | Type | Description |
|-----------|------|-------------|
| `email` | `string` | Email assigned to the owner's Tesla account. |
| `password` | `string` | Password for the given Tesla account. |
| `multiFactorAuthResolver` | `Func<(string passcode, string backupPasscode)>` | Callback function used if multi-factor authentication is turned on for the given account. Defaults to `null`. |

The method returns an `AccessToken` which includes several details returned by the authentication process. The most important properties are the `Token` and `RefreshToken` which are used to access the rest of the APIs.

```csharp
public sealed class AccessToken
{
    /// <summary>
    /// Time this token was created
    /// </summary>
    public DateTime CreatedTimeUtc { get; set; }
    /// <summary>
    /// Time this token expires
    /// </summary>
    public DateTime ExpirationTimeUtc { get; set; }
    /// <summary>
    /// The access token to save off
    /// </summary>
    public string Token { get; set; }
    /// <summary>
    /// Refresh token to refresh access.
    /// </summary>
    public string RefreshToken { get; set; }
}
```

Once you obtain the access token information, you should store it off for future use. Tokens are generally good for up to 45 days - you can determine the exact availability using the returned `ExpirationTimeUtc` property. See below for the process on refreshing an access token.

### Logging into the Tesla API

Here's an example:

```csharp
var client = new TeslaClient();

var accessToken = await client.LoginAsync("elon@tesla.com", "SpaceX!2021");

// TODO: store off the data returned somewhere safe. These are keys which anyone can use
// to access the API using your account!
```

> **Note**
>
> The API uses a hardcoded ClientId and ClientSecret to authenticate with Tesla. This is taken from the existing Tesla app and essentially
> represents a fixed secret used to identify the app. So far, this value has been valid and allowed by anyone who hits the REST APIs directly,
> however, it's possible it could be invalidated in the future and replaced with different values. If this happens, you can _change_ the hardcoded
> values used by this library through the `ClientId` and `ClientSecret` properties. Set proper values _before_ calling `LoginAsync`.
>
> These properties are in place just in case this fixed value ever changes. As of today, you can ignore them and leave the default values in place.

### Multi-factor authentication

If multi-factor authentication (MFA) is turned on for your account, you will need to provide an additional passcode from an authenticator app or backup passcode generated by Tesla. These values can be provided to the login sequence through the optional `multiFactorAuthResolver` parameter. This is a delegate function that should return _either_ a just-in-time passcode from the auth app, or a one-time backup passcode. The client application is responsible for collecting this information from the user through some interactive session. Here's an example:

```csharp
var accessToken = await client.LoginAsync("elon@tesla.com", "SpaceX!2021",
 () => {

    // return a backup passcode - this can only be used once!
    // but once we have a token, we can continue to refresh and won't need
    // client credentials again.
    return (null, "mybackup-passcode-here");
    // Alternatively, you could pass a just-in-time auth-generated passcode in the first
    // tuple field. This would be useful if the app is a client-facing app that can ask
    // the account owner for the code in real-time.
});

```

### Signing in with an access token

Once you have an access token, you can cache it off and then re-create the `TeslaClient` object in the future from that cached value through the static `CreateFromToken` method.

```csharp
string accessToken = GetAccessTokenFromSecureStore(); // TODO: you have to provide storage
var client = TeslaClient.CreateFromToken(accessToken);
...
```

### Refreshing the access token

The returned access token appears to be valid for a little over a month. Once it expires (based on the returned expiration date), you will need to refresh it. By default, APIs will fail and throw an `InvalidTokenException` when the token is invalidated. There are two ways you can refresh the token. The first is to go back through the `LoginAsync` method and generate a new token. This is the _only_ mechanism available if you don't have a valid `refreshToken`.

With a refresh token, you can use the `RefreshLoginAsync` method to retrieve a new `AccessToken` object.

```csharp
string accessToken = ...;   // get from secure storage
string refreshToken = ...;  // get from secure storage
DateTime expireTimeUtc = ...;

TeslaClient client;

if (expireTimeUtc < DateTime.NowUtc.AddDays(1))
{
    // Still have a day left!
    client = TeslaClient.CreateFromToken(accessToken);
}
else
{
    // Close or expired.
    client = new TeslaClient();
    AccessToken tokenData = await client.RefreshLoginAsync(refreshToken);
    // TODO: store access token off. Client is already refreshed.
}

// Can use client
```

#### Auto-refresh the token

To do just-in-time refresh, you can have the API auto-refresh the token. This is done by assigning a delegate to the `AutoRefresh` property. This will be called automatically if the token is invalid. From the client-side, you won't even notice that the API failed. Here's an example of this method:

```csharp
string accessToken = ...;   // get from secure storage
string refreshToken = ...;  // get from secure storage

var client = TeslaClient.CreateFromToken(accessToken);

// This is called when an invalid token is detected. You can 
// use the API as shown here, or use a different technique to obtain a valid
// token and return it from the delegate.
client.AutoRefreshToken = async () => 
     // Refresh the token
     AccessToken tokenData = await client.RefreshLoginAsync(refreshToken);
     // TODO: store token data off.
     // Return the new access token
     return tokenData.Token;
```

### Revoking a token

Finally, you can revoke an active token through the `RevokeTokenAsync` method.

```csharp
string accessToken = ...;   // get from secure storage

var client = new TeslaClient();
await client.RevokeTokenAsync(accessToken);

```

## Debugging the APIs

If you are interested in the underlying API and JSON responses, you can set the `TraceLog` property to a valid callback. This is invoked anytime the library interacts with the Tesla API. It is passed a `LogLevel` and a `string` text value. The logging level indicates the type of data being reported on:

```csharp
/// <summary>
/// Log level for the <see cref="TeslaClient.TraceLog"/> property.
/// </summary>
[Flags]
public enum LogLevel
{
    /// <summary>
    /// Information and state data
    /// </summary>
    Info = 1,
    /// <summary>
    /// HTTP queries
    /// </summary>
    Query = 2,
    /// <summary>
    /// HTTP responses (header + status)
    /// </summary>
    Response = 4,
    /// <summary>
    /// JSON data responses
    /// </summary>
    RawData = 8
}
```

Your app can decide what to emit based on the value of this flag. Here's an example of setting up a logger:

```csharp
var client = TeslaClient.CreateFromToken(...)

// Log all internal things to the console
client.TraceLog = (level, text) => Console.WriteLine($"{level}: {text}");
```

## Retrieving vehicle data

Once you have a `TeslaClient`, you can retrieve a `Vehicle`. There are two methods to do this:

1. `GetVehiclesAsync` returns a list of all vehicles associated with the account.
1. `GetVehicleAsync` returns a _single_ vehicle using the uniquely assigned id.

Once you have a `Vehicle`, you can retrieve data or execute commands against it. Here's the definition for a vehicle:

```csharp
/// <summary>
/// Represents a single Tesla vehicle tied to a Tesla account.
/// This is the entrypoint for all of the status and command APIs.
/// </summary>
public sealed class Vehicle
{
    /// <summary>
    /// Unique identifier on this account for this vehicle.
    /// This is what should be used to identify the vehicle to the API
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Vehicle Id from Tesla
    /// </summary>
    public long VehicleId { get; set; }
    
    /// <summary>
    /// Assigned VIN for the car.
    /// </summary>
    public string VIN { get; set; }
    
    /// <summary>
    /// Display name (assigned by owner)
    /// </summary>
    public string DisplayName { get; set; }
    
    /// <summary>
    /// Option codes (not reliable)
    /// </summary>
    public string OptionCodes { get; set; }

    /// <summary>
    /// Color of vehicle. Not always present, use GetVehicleData to retrieve more details.
    /// </summary>
    public string Color { get; set; }
    
    /// <summary>
    /// Current state of the car
    /// </summary>
    public string State { get; set; }
    
    /// <summary>
    /// True if the car is in service.
    /// </summary>
    public bool InService { get; set; }
    
    /// <summary>
    /// True if the calendar is enabled.
    /// </summary>
    public bool CalendarEnabled { get; set; }
    
    /// <summary>
    /// API version
    /// </summary>
    public int ApiVersion { get; set; }

    /// <summary>
    /// Wakes up the car from a sleeping state.
    /// The API will return a response immediately, however it could take several seconds before the car
    /// is actually online and ready to receive other commands. You can call this API with a 30-second delay
    /// until it returns 'true'.
    /// </summary>
    /// <returns>True if the car is online</returns>
    public async Task<bool> WakeupAsync();

    /// <summary>
    /// Information on the state of charge in the battery and its various settings.
    /// </summary>
    /// <returns>Charge state object</returns>
    public Task<ChargeState> GetChargeStateAsync();

    /// <summary>
    /// Information on the climate settings.
    /// </summary>
    /// <returns>Climate state object</returns>
    public Task<ClimateState> GetClimateStateAsync();

    /// <summary>
    /// Returns the driving and position state of the vehicle.
    /// </summary>
    /// <returns>Drive status object</returns>
    public Task<DriveState> GetDriveStateAsync();

    /// <summary>
    /// True if mobile access setting is enabled on the car.
    /// </summary>
    /// <returns></returns>
    public Task<bool> IsMobileAccessEnabledAsync();

    /// <summary>
    /// Retrieve the user settings related to the touchscreen interface such as driving units and locale.
    /// </summary>
    public Task<GuiSettings> GetGuiSettingsAsync();

    /// <summary>
    /// Returns the vehicle's physical state, such as which doors are open.
    /// </summary>
    public Task<VehicleState> GetVehicleStateAsync();

    /// <summary>
    /// Returns the vehicle's configuration information including model, color, badging and wheels.
    /// </summary>
    public Task<VehicleConfiguration> GetVehicleConfigurationAsync();

    /// <summary>
    /// Returns a list of nearby Tesla-operated charging stations. (Requires car software version 2018.48 or higher.)
    /// </summary>
    public Task<ChargingStations> GetNearbyChargingStationsAsync();

    /// <summary>
    /// A rollup of all the data_request endpoints plus vehicle configuration.
    /// </summary>
    public Task<VehicleDataRollup> GetAllVehicleDataAsync();

    /// <summary>
    /// Honk the horn.
    /// </summary>
    /// <returns>True if success.</returns>
    public Task<bool> HonkHornAsync();

    /// <summary>
    /// Flash the headlights.
    /// </summary>
    /// <returns>True if success.</returns>
    public Task<bool> FlashLightsAsync();

    /// <summary>
    /// Attempt to start the car and allow a person without a key to drive the car.
    /// Once this returns, the person has 2-minutes to get into the car and drive it.
    /// </summary>
    /// <param name="accountPassword">The password for the authenticated tesla.com account.</param>
    /// <returns>True if success.</returns>
    public Task<bool> RemoteStartAsync(string accountPassword);

    /// <summary>
    /// Lock the doors
    /// </summary>
    /// <returns>True if success.</returns>
    public Task<bool> LockDoorsAsync();

    /// <summary>
    /// Unlock the doors.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> UnlockDoorsAsync();

    /// <summary>
    /// Opens or closes the primary Homelink device.
    /// The provided location must be in proximity of stored location of the Homelink device.
    /// </summary>
    /// <param name="latitude">Current latitude.</param>
    /// <param name="longitude">Current longitude.</param>
    /// <returns>True on success.</returns>
    public Task<bool> TriggerHomelinkAsync(double latitude, double longitude);

    /// <summary>
    /// Sets the maximum speed allowed when Speed Limit Mode is active.
    /// </summary>
    /// <param name="mph">The speed limit in MPH. Must be between 50-90.</param>
    /// <returns>True on success.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Task<bool> SetSpeedLimitAsync(int mph);

    /// <summary>
    /// Set vehicle valet mode on or off with a PIN to disable it from within the car.
    /// Reuses last PIN from previous valet session. Valet Mode limits the car's top speed to 70MPH
    /// and 80kW of acceleration power. It also disables Homelink, Bluetooth and Wifi settings,
    /// and the ability to disable mobile access to the car. It also hides your favorites, home,
    /// and work locations in navigation.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> ActivateValetModeAsync();
    
    /// <summary>
    /// Deactivates Sentry Mode. If a PIN code is required, it must be supplied.
    /// </summary>
    /// <param name="pinCode">Optional 4-digit PIN to deactivate valet mode</param>
    /// <returns>True on success.</returns>
    public Task<bool> DeactivateValetModeAsync(string pinCode = null);

    /// <summary>
    /// Clears the currently set PIN for Valet Mode when deactivated. A new PIN will be required when
    /// activating from the car screen.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> ResetValetModePinAsync();

    /// <summary>
    /// Turn sentry mode on.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> ActivateSentryModeAsync();
    
    /// <summary>
    /// Turn sentry mode off.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> DeactivateSentryModeAsync();

    /// <summary>
    /// Opens the trunk
    /// </summary>
    /// <returns>True on success</returns>
    public Task<bool> OpenTrunkAsync();

    /// <summary>
    /// Opens the front trunk (frunk)
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> OpenFrunkAsync();

    /// <summary>
    /// Vents the windows.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> VentWindowsAsync();
    
    /// <summary>
    /// Closes the windows. Latitude and longitude values must be near the current location of the car for close operation to succeed.
    /// </summary>
    /// <param name="latitude">Current latitude.</param>
    /// <param name="longitude">Current longitude.</param>
    /// <returns>True on success.</returns>
    public Task<bool> CloseWindowsAsync(double latitude, double longitude);

    /// <summary>
    /// Vents the panoramic sunroof on the Model S.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> VentSunroofAsync();
    
    /// <summary>
    /// Closes the panoramic sunroof on the Model S.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> CloseSunroofAsync();

    /// <summary>
    /// Opens/Unlocks the charging port.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> OpenChargingPortAsync();

    /// <summary>
    /// Closes the charging port on cars with a motorized charging port.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> CloseChargingPortAsync();

    /// <summary>
    /// If the car is plugged in but not currently charging, this will start it charging.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> StartChargingAsync();

    /// <summary>
    /// If the car is currently charging, this will stop it.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> StopChargingAsync();

    /// <summary>
    /// If the car is currently charging, this will stop it.
    /// </summary>
    /// <param name="percentage">Percentage to set to. If omitted, will use 'standard' limit.</param>
    /// <returns>True on success.</returns>
    public Task<bool> SetChargeLimitAsync(int? percentage = null);

    /// <summary>
    /// Start the climate control (HVAC) system. Will cool or heat automatically, depending on set temperature.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> StartClimateControlAsync();
    
    /// <summary>
    /// Stop the climate control (HVAC) system.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> StopClimateControlAsync();

    /// <summary>
    /// Sets the target temperature for the climate control (HVAC) system.
    /// </summary>
    /// <param name="celsiusTemp">The desired temperature in celsius.</param>
    /// <returns>True on success.</returns>
    public Task<bool> SetClimateControlTemperatureAsync(double celsiusTemp);
    
    /// <summary>
    /// Turn on the max defrost setting.
    /// </summary>
    /// <returns>True on success</returns>
    public Task<bool> StartClimateControlDefrostAsync();

    /// <summary>
    /// Turn off the max defrost setting. Switches climate control back to default setting.
    /// </summary>
    /// <returns>True on success</returns>
    public Task<bool> StopClimateControlDefrostAsync();

    /// <summary>
    /// Sets the specified seat's heater level.
    /// </summary>
    /// <param name="seat">The desired seat to heat.</param>
    /// <param name="value">The desired level for the heater.</param>
    /// <returns>True on success.</returns>
    public Task<bool> SetSeatHeaterAsync(Seat seat, SeatHeater value);

    /// <summary>
    /// Turn steering wheel heater on or off.
    /// </summary>
    /// <param name="onOff"></param>
    /// <returns>True on success.</returns>
    public Task<bool> SetSteeringWheelHeaterAsync(bool onOff);

    /// <summary>
    /// Toggles the media between playing and paused. For the radio, this mutes or unmutes the audio.
    /// The car must be on.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> MediaTogglePlayPauseAsync();

    /// <summary>
    /// Skips to the next track in the current playlist, or to the next station.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> MediaNextTrackAsync();

    /// <summary>
    /// Skips to the previous track in the current playlist, or to the next station. Does nothing for streaming stations.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> MediaPreviousTrackAsync();

    /// <summary>
    /// Skips to the next saved favorite in the media system.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> MediaNextFavoriteAsync();
    
    /// <summary>
    /// Skips to the previous saved favorite in the media system.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> MediaPreviousFavoriteAsync();
    
    /// <summary>
    /// Turns up the volume of the media system.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> MediaVolumeUpAsync();
    
    /// <summary>
    /// Turns down the volume of the media system.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> MediaVolumeDownAsync();
    
    /// <summary>
    /// Schedules a software update to be installed, if one is available.
    /// </summary>
    /// <param name="delayInSeconds">How many seconds in the future to schedule the update. Set to 0 for immediate install.</param>
    /// <returns>True on success.</returns>
    public Task<bool> ScheduleSoftwareUpdateAsync(int delayInSeconds = 0);
    
    /// <summary>
    /// Cancels a software update, if one is scheduled and has not yet started.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> CancelSoftwareUpdateAsync();

    /// <summary>
    /// Set upcoming calendar entries.
    /// </summary>
    /// <returns>True on success.</returns>
    public Task<bool> SetUpcomingCalendarEntriesAsync();

    /// <summary>
    /// Sends a location for the car to start navigation.
    /// </summary>
    /// <param name="address">Address to navigate to</param>
    /// <param name="locale">Locale of address - if not supplied, current UI culture is used.</param>
    /// <returns>True on success.</returns>
    public Task<bool> ShareAddressForNavigationAsync(string address, string locale = null);

    /// <summary>
    /// Play a video in theatre mode.
    /// </summary>
    /// <param name="uri">Web address for video to play</param>
    /// <returns>True on success.</returns>
    public Task<bool> PlayFullscreenVideoAsync(Uri uri)å;
}
```

## Example code

Here's an example .NET console application that uses the API.

```csharp
var api = new TeslaClient();

string email = "elon@tesla.com";
string password = "spacex-roxx";

var tokenResponse = await api.LoginAsync(email, password);

// Get all vehicles on this account
var allCars = await api.GetVehiclesAsync();
foreach (var item in allCars)
    Console.WriteLine(item);

// Here's how to retrieve by ID
var myCar = await api.GetVehicleAsync(allCars.First().Id);

// Wake the car up.
if (!myCar.IsAwake)
{
    if (!await myCar.WakeupAsync(30))
    {
        Console.WriteLine("Unable to wake up car.");
        return;
    }
}

// Dump out common state values
Console.WriteLine(await myCar.GetChargeStateAsync());
Console.WriteLine(await myCar.GetClimateStateAsync());
Console.WriteLine(await myCar.GetDriveStateAsync());
Console.WriteLine(await myCar.GetGuiSettingsAsync());
Console.WriteLine(await myCar.GetVehicleStateAsync());
Console.WriteLine(await myCar.GetVehicleConfigurationAsync());

// Nearby charging stations - you could build a "Charging Station" map from this data.
var nearbyChargers = await myCar.GetNearbyChargingStationsAsync();
Console.WriteLine(nearbyChargers);
Console.WriteLine(nearbyChargers.DestinationChargers.FirstOrDefault()?.ToString());
Console.WriteLine(nearbyChargers.Superchargers.FirstOrDefault()?.ToString());

// This is the "all data" API which retrieves charge, climate, drive, Gui, vehicle, etc. all in one go.
var allVehicleData = await myCar.GetAllVehicleDataAsync();
Console.WriteLine(allVehicleData.VIN);

// These are some simple commands.
await myCar.HonkHornAsync();
await myCar.FlashLightsAsync();
```
