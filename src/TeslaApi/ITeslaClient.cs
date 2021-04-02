using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeslaApi.Models;

namespace TeslaApi
{
    public interface ITeslaClient
    {
        string Email { get; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string AccessToken { get; }
        Action<string> TraceLog { get; set; }

        Task<ChargeState> GetChargeStateAsync(long id);
        Task<ClimateState> GetClimateStateAsync(long id);
        Task<DriveState> GetDriveStateAsync(long id);
        Task<GuiSettings> GetGuiSettingsAsync(long id);
        Task<Vehicle> GetVehicleAsync(long id);
        Task<IReadOnlyList<Vehicle>> GetVehiclesAsync();
        Task<VehicleState> GetVehicleStateAsync(long id);
        Task<bool> IsMobileAccessEnabledAsync(long id);
        Task LoginAsync(string email, string password, Func<(string passcode, string backupPasscode)> multiFactorAuthResolver = null);
        Task<Vehicle> WakeupAsync(long id);
        Task<VehicleConfiguration> GetVehicleConfigurationAsync(long id);
        Task<ChargingStations> GetNearbyChargingStations(long id);
    }
}