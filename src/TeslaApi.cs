using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using TeslaApp.Models;

namespace TeslaApi
{
    public class TeslaClient
    {
        const string OwnerApiEndpoint = "https://owner-api.teslamotors.com/api/";
        private string accessToken;
        private string refreshToken;
        private HttpClient client;

        private TeslaClient()
        {
            ClientId = Internal.Auth.TESLA_CLIENT_ID;
            ClientSecret = Internal.Auth.TESLA_CLIENT_SECRET;
        }

        public string Email { get; private set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Action<string> TraceLog;

        private HttpClient Client
        {
            get
            {
                if (client == null)
                {
                    HttpClientHandler handler = new() { AllowAutoRedirect = false, CookieContainer = new CookieContainer() };
                    client = new HttpClient(handler);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }

                return client;
            }
        }

        private async Task<T> GetOneAsync<T>(string endpoint)
        {
            string url = OwnerApiEndpoint + endpoint;
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
            string url = OwnerApiEndpoint + endpoint;
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
            string url = OwnerApiEndpoint + endpoint;
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

        public static TeslaClient Create()
        {
            return new TeslaClient();
        }

        public static TeslaClient CreateFromToken(string token)
        {
            return new TeslaClient {
                accessToken = token,
            };
        }

        public async Task LoginAsync(string email, string password,Func<(string passcode, string backupPasscode)> multiFactorAuthResolver = null)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException($"'{nameof(email)}' cannot be null or empty.", nameof(email));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));

            (accessToken, refreshToken) = await Internal.Auth.GetAccessTokenAsync(email, password, multiFactorAuthResolver);
        }

        public Task<IReadOnlyList<Vehicle>> GetVehiclesAsync() => GetListAsync<Vehicle>("1/vehicles");
        public Task<Vehicle> GetVehicleAsync(long id) => GetOneAsync<Vehicle>($"1/vehicles/{id}");
        public Task<Vehicle> Wakeup(long id) => PostOneAsync<Vehicle>($"1/vehicles/{id}/wake_up");

        public Task<ChargeState> GetChargeState(long id) => GetOneAsync<ChargeState>($"1/vehicles/{id}/data_request/charge_state");
        public Task<ClimateState> GetClimateState(long id) => GetOneAsync<ClimateState>($"1/vehicles/{id}/data_request/climate_state");
        public Task<DriveState> GetDriveState(long id) => GetOneAsync<DriveState>($"1/vehicles/{id}/data_request/drive_state");
    }
}
