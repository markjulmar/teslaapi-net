using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using TeslaApi.Internal;
using TeslaApi.Models;

namespace TeslaApi
{
    public class TeslaClient
    {
        public static ITeslaClient Create()
        {
            return new TeslaClientImpl();
        }

        public static ITeslaClient CreateFromToken(string token)
        {
            return new TeslaClientImpl(token);
        }
    }

    class TeslaClientImpl : ITeslaClient
    {
        private string refreshToken;
        private HttpClient client;

        internal TeslaClientImpl(string accessToken = null)
        {
            AccessToken = accessToken;
            ClientId = Constants.TESLA_CLIENT_ID;
            ClientSecret = Constants.TESLA_CLIENT_SECRET;
        }

        public string Email { get; private set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Action<string> TraceLog { get; set; }

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
            TraceLog?.Invoke($"=> {url}");
            var result = await Client.GetAsync(url);
            TraceLog?.Invoke($"<=\r\n{result}");

            if (!result.IsSuccessStatusCode)
            {
                result.Content?.Dispose();
                if (result.StatusCode == HttpStatusCode.RequestTimeout)
                    throw new SleepingException();
            }

            return JsonSerializer.Deserialize<OneResponse<T>>(await result.Content.ReadAsStringAsync()).Response;
        }

        private async Task<T> PostOneAsync<T>(string endpoint)
        {
            string url = Constants.VehiclesApi + endpoint;
            TraceLog?.Invoke($"=> {url}");
            var result = await Client.PostAsync(url, new StringContent(""));
            TraceLog?.Invoke($"<=\r\n{result}");

            if (!result.IsSuccessStatusCode)
            {
                result.Content?.Dispose();
                if (result.StatusCode == HttpStatusCode.RequestTimeout)
                    throw new SleepingException();
            }

            return JsonSerializer.Deserialize<OneResponse<T>>(await result.Content.ReadAsStringAsync()).Response;
        }

        private async Task<IReadOnlyList<T>> GetListAsync<T>(string endpoint)
        {
            string url = Constants.VehiclesApi + endpoint;
            TraceLog?.Invoke($"=> {url}");
            var result = await Client.GetAsync(url);
            TraceLog?.Invoke($"<=\r\n{result}");

            if (!result.IsSuccessStatusCode)
            {
                result.Content?.Dispose();
                if (result.StatusCode == HttpStatusCode.RequestTimeout)
                    throw new SleepingException();
            }

            return JsonSerializer.Deserialize<ListResponse<T>>(await result.Content.ReadAsStringAsync()).Response;
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
                ClientId, ClientSecret,
                multiFactorAuthResolver);
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
    }
}
