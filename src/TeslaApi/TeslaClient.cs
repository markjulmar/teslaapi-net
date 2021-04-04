using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Julmar.TeslaApi.Internal;

namespace Julmar.TeslaApi
{
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
        /// HTTP responses
        /// </summary>
        Response = 4,
        /// <summary>
        /// JSON data responses
        /// </summary>
        RawData = 8
    }
    
    /// <summary>
    /// Main Tesla client API
    /// </summary>
    public class TeslaClient : IDisposable
    {
        private string refreshToken;
        private HttpClient client;
        private JsonSerializerOptions serializerOptions;

        /// <summary>
        /// Constructor for the TeslaClient.
        /// </summary>
        public TeslaClient()
        {
            ClientId = Constants.TESLA_CLIENT_ID;
            ClientSecret = Constants.TESLA_CLIENT_SECRET;
            serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            serializerOptions.Converters.Add(new IntToBoolJsonConverter());
        }
        
        /// <summary>
        /// Internal constructor built around an access token.
        /// </summary>
        /// <param name="accessToken">Tesla owner account access token</param>
        private TeslaClient(string accessToken = null) : this()
        {
            AccessToken = accessToken;
        }
        
        /// <summary>
        /// Create a TeslaClient from an access token.
        /// </summary>
        /// <param name="token">Tesla owner account access token.</param>
        /// <returns>TeslaAccount object</returns>
        public static TeslaClient CreateFromToken(string token)
        {
            return new (token);
        }
        
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
        /// Underlying web connection. Can be created as needed.
        /// </summary>
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

        /// <summary>
        /// Retrieve a single value from the Tesla API.
        /// </summary>
        /// <param name="endpoint">Endpoint to call</param>
        /// <typeparam name="T">Returning type</typeparam>
        /// <returns>Return value from API</returns>
        /// <exception cref="SleepingException">Car is asleep and not accepting commands.</exception>
        internal async Task<T> GetOneAsync<T>(string endpoint)
        {
            string url = Constants.VehiclesApi + endpoint;
            TraceLog?.Invoke(LogLevel.Query, $"GET {url}");
            var result = await Client.GetAsync(url);
            TraceLog?.Invoke(LogLevel.Response, result.ToString());

            if (!result.IsSuccessStatusCode)
            {
                result.Content?.Dispose();
                switch (result.StatusCode)
                {
                    case HttpStatusCode.RequestTimeout:
                        throw new SleepingException();
                    case HttpStatusCode.NotFound:
                        return default;
                }
                result.EnsureSuccessStatusCode();
            }

            string contents = await result.Content.ReadAsStringAsync();
            TraceLog?.Invoke(LogLevel.RawData, contents);
            return JsonSerializer.Deserialize<OneResponse<T>>(contents, serializerOptions).Response;
        }

        /// <summary>
        /// Post a command with a boolean response.
        /// </summary>
        /// <param name="endpoint">Endpoint</param>
        /// <returns>True/False</returns>
        internal async Task<bool> PostCommandAsync(string endpoint) =>
            (await PostOneAsync<OneResponse<CommandResponse>>(endpoint))
            .Response.Result;

        /// <summary>
        /// POST a command and retrieve a single value from the Tesla API.
        /// </summary>
        /// <param name="endpoint">Endpoint to call</param>
        /// <typeparam name="T">Returning type</typeparam>
        /// <returns>Return value from API</returns>
        /// <exception cref="SleepingException">Car is asleep and not accepting commands.</exception>
        internal async Task<T> PostOneAsync<T>(string endpoint)
        {
            string url = Constants.VehiclesApi + endpoint;
            TraceLog?.Invoke(LogLevel.Query, $"POST {url}");
            var result = await Client.PostAsync(url, new StringContent(""));
            TraceLog?.Invoke(LogLevel.Response, result.ToString());

            if (!result.IsSuccessStatusCode)
            {
                result.Content?.Dispose();
                switch (result.StatusCode)
                {
                    case HttpStatusCode.RequestTimeout:
                        throw new SleepingException();
                    case HttpStatusCode.NotFound:
                        return default;
                }
                result.EnsureSuccessStatusCode();
            }

            string contents = await result.Content.ReadAsStringAsync();
            TraceLog?.Invoke(LogLevel.RawData, contents);
            return JsonSerializer.Deserialize<OneResponse<T>>(contents, serializerOptions).Response;
        }

        /// <summary>
        /// Retrieve a set of values from the Tesla API.
        /// </summary>
        /// <param name="endpoint">Endpoint to call</param>
        /// <typeparam name="T">Returning type</typeparam>
        /// <returns>Return value from API</returns>
        /// <exception cref="SleepingException">Car is asleep and not accepting commands.</exception>
        internal async Task<IReadOnlyList<T>> GetListAsync<T>(string endpoint)
        {
            string url = Constants.VehiclesApi + endpoint;
            TraceLog?.Invoke(LogLevel.Query, $"GET {url}");
            var result = await Client.GetAsync(url);
            TraceLog?.Invoke(LogLevel.Response, result.ToString());

            if (!result.IsSuccessStatusCode)
            {
                result.Content?.Dispose();
                switch (result.StatusCode)
                {
                    case HttpStatusCode.RequestTimeout:
                        throw new SleepingException();
                    case HttpStatusCode.NotFound:
                        return null;
                }
            }

            string contents = await result.Content.ReadAsStringAsync();
            TraceLog?.Invoke(LogLevel.RawData, contents);
            return JsonSerializer.Deserialize<ListResponse<T>>(contents, serializerOptions).Response;
        }

        /// <summary>
        /// Login to the Tesla API and retrieve an owner API token.
        /// </summary>
        /// <param name="email">Email account tied to Tesla</param>
        /// <param name="password">Password for the Tesla account</param>
        /// <param name="multiFactorAuthResolver">Optional resolver for Multi-Factor auth.</param>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Retrieve a list of all the vehicles tied to this account.
        /// </summary>
        /// <returns>List of vehicles.</returns>
        public async Task<IReadOnlyList<Vehicle>> GetVehiclesAsync()
        {
            var results = await GetListAsync<Vehicle>(string.Empty);
            foreach (var vehicle in results)
                vehicle.SetClient(this);
            return results;
        }

        /// <summary>
        /// Return a single vehicle object based on the unique identifier.
        /// </summary>
        /// <param name="id">Id of the car</param>
        /// <returns>Vehicle object</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task<Vehicle> GetVehicleAsync(long id)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
            var vehicle = await GetOneAsync<Vehicle>(id.ToString());
            vehicle?.SetClient(this);
            return vehicle;
        }

        /// <summary>
        /// Dispose the underlying connection. Note that it can be
        /// recreated by an attached vehicle.
        /// </summary>
        public void Dispose()
        {
            client?.Dispose();
            client = null;
        }
    }
}
