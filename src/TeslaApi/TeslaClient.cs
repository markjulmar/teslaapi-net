using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Julmar.TeslaApi.Internal;
using Julmar.TeslaApi.Models;

namespace Julmar.TeslaApi
{
    [Flags]
    public enum LogLevel
    {
        None = 0,
        Info = 1,
        Query = 2,
        Response = 4,
        RawData = 8,
        All = Info | Query | Response | RawData
    }
    
    public class TeslaClient
    {
        private string refreshToken;
        private HttpClient client;

        public TeslaClient()
        {
            ClientId = Constants.TESLA_CLIENT_ID;
            ClientSecret = Constants.TESLA_CLIENT_SECRET;
        }
        private TeslaClient(string accessToken = null) : this()
        {
            AccessToken = accessToken;
        }
        
        public static TeslaClient CreateFromToken(string token)
        {
            return new TeslaClient(token);
        }
        
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Action<LogLevel, string> TraceLog { get; set; }

        private HttpClient Client
        {
            get
            {
                if (client == null)
                {
                    if (AccessToken == null)
                        throw new Exception("Must use Login method before using any API methods.");

                    HttpClientHandler handler = new() { AllowAutoRedirect = false, CookieContainer = new CookieContainer() };
                    client = new HttpClient(handler);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                }

                return client;
            }
        }

        /// <summary>
        /// The Tesla access token being used.
        /// </summary>
        public string AccessToken { get; private set; }

        private async Task<T> GetOneAsync<T>(string endpoint)
        {
            string url = Constants.VehiclesApi + endpoint;
            TraceLog?.Invoke(LogLevel.Query, $"GET {url}");
            var result = await Client.GetAsync(url);
            TraceLog?.Invoke(LogLevel.Response, result.ToString());

            if (!result.IsSuccessStatusCode)
            {
                result.Content?.Dispose();
                if (result.StatusCode == HttpStatusCode.RequestTimeout)
                    throw new SleepingException();
                result.EnsureSuccessStatusCode();
            }

            string contents = await result.Content.ReadAsStringAsync();
            TraceLog?.Invoke(LogLevel.RawData, contents);
            return JsonSerializer.Deserialize<OneResponse<T>>(contents).Response;
        }

        private async Task<T> PostOneAsync<T>(string endpoint)
        {
            string url = Constants.VehiclesApi + endpoint;
            TraceLog?.Invoke(LogLevel.Query, $"POST {url}");
            var result = await Client.PostAsync(url, new StringContent(""));
            TraceLog?.Invoke(LogLevel.Response, result.ToString());

            if (!result.IsSuccessStatusCode)
            {
                result.Content?.Dispose();
                if (result.StatusCode == HttpStatusCode.RequestTimeout)
                    throw new SleepingException();
                result.EnsureSuccessStatusCode();
            }

            string contents = await result.Content.ReadAsStringAsync();
            TraceLog?.Invoke(LogLevel.RawData, contents);
            return JsonSerializer.Deserialize<OneResponse<T>>(contents).Response;
        }

        private async Task<IReadOnlyList<T>> GetListAsync<T>(string endpoint)
        {
            string url = Constants.VehiclesApi + endpoint;
            TraceLog?.Invoke(LogLevel.Query, $"GET {url}");
            var result = await Client.GetAsync(url);
            TraceLog?.Invoke(LogLevel.Response, result.ToString());

            if (!result.IsSuccessStatusCode)
            {
                result.Content?.Dispose();
                if (result.StatusCode == HttpStatusCode.RequestTimeout)
                    throw new SleepingException();
            }

            string contents = await result.Content.ReadAsStringAsync();
            TraceLog?.Invoke(LogLevel.RawData, contents);
            return JsonSerializer.Deserialize<ListResponse<T>>(contents).Response;
        }

        public async Task LoginAsync(string email, string password, Func<(string passcode, string backupPasscode)> multiFactorAuthResolver = null)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException($"'{nameof(email)}' cannot be null or empty.", nameof(email));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            if (string.IsNullOrEmpty(ClientId))
                throw new ArgumentException($"'{nameof(ClientId)}' cannot be null or empty.", nameof(password));
            if (string.IsNullOrEmpty(ClientSecret))
                throw new ArgumentException($"'{nameof(ClientSecret)}' cannot be null or empty.", nameof(password));

            (AccessToken, refreshToken) = await Auth.GetAccessTokenAsync(email, password,
                ClientId, ClientSecret, TraceLog, multiFactorAuthResolver);
        }

        public Task<IReadOnlyList<Vehicle>> GetVehiclesAsync() => GetListAsync<Vehicle>(string.Empty);
        public Task<Vehicle> GetVehicleAsync(long id) => GetOneAsync<Vehicle>(id.ToString());
        public Task<Vehicle> WakeupAsync(long id) => PostOneAsync<Vehicle>($"{id}/wake_up");

        public Task<ChargeState> GetChargeStateAsync(long id) => GetOneAsync<ChargeState>($"{id}/data_request/charge_state");
        public Task<ClimateState> GetClimateStateAsync(long id) => GetOneAsync<ClimateState>($"{id}/data_request/climate_state");
        public Task<DriveState> GetDriveStateAsync(long id) => GetOneAsync<DriveState>($"{id}/data_request/drive_state");
        public Task<bool> IsMobileAccessEnabledAsync(long id) => GetOneAsync<bool>($"{id}/mobile_enabled");
        public Task<GuiSettings> GetGuiSettingsAsync(long id) => GetOneAsync<GuiSettings>($"{id}/data_request/gui_settings");
        public Task<VehicleState> GetVehicleStateAsync(long id) => GetOneAsync<VehicleState>($"{id}/data_request/vehicle_state");
        public Task<VehicleConfiguration> GetVehicleConfigurationAsync(long id) => GetOneAsync<VehicleConfiguration>($"{id}/data_request/vehicle_config");
        public Task<ChargingStations> GetNearbyChargingStations(long id) => GetOneAsync<ChargingStations>($"{id}/nearby_charging_sites");
        public Task<VehicleDataRollup> GetAllVehicleDataAsync(long id) => GetOneAsync<VehicleDataRollup>($"{id}/vehicle_data");
    }
}
