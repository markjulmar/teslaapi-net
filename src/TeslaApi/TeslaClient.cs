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
        private string accessToken;
        private HttpClient client;

        /// <summary>
        /// Constructor for the TeslaClient.
        /// </summary>
        public TeslaClient()
        {
            ClientId = Constants.TESLA_CLIENT_ID;
            ClientSecret = Constants.TESLA_CLIENT_SECRET;
        }
        
        /// <summary>
        /// Internal constructor built around an access token.
        /// </summary>
        /// <param name="accessToken">Tesla owner account access token</param>
        private TeslaClient(string accessToken) : this()
        {
            this.accessToken = accessToken;
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
        /// Function to auto-refresh token and reissue any failing call.
        /// </summary>
        public Func<AccessToken> AutoRefreshToken { get; set; }

        /// <summary>
        /// Underlying web connection. Can be created as needed.
        /// </summary>
        private HttpClient Client
        {
            get
            {
                if (client == null)
                {
                    if (accessToken == null)
                        throw new Exception("Must use Login method before using any API methods.");

                    HttpClientHandler handler = new() { AllowAutoRedirect = false, CookieContainer = new CookieContainer() };
                    client = new HttpClient(handler);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }

                return client;
            }
        }

        /// <summary>
        /// Retrieve a single value from the Tesla API.
        /// </summary>
        /// <param name="endpoint">Endpoint to call</param>
        /// <param name="isRetryWithNewAccessToken">True if this is a recursive call</param>
        /// <typeparam name="T">Returning type</typeparam>
        /// <returns>Return value from API</returns>
        /// <exception cref="SleepingException">Car is asleep and not accepting commands.</exception>
        internal async Task<T> GetOneAsync<T>(string endpoint, bool isRetryWithNewAccessToken = false)
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
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.Unauthorized:
                        if (!isRetryWithNewAccessToken && AutoRefreshToken != null)
                        {
                            var authResponse = AutoRefreshToken.Invoke();
                            if (authResponse?.Token != null)
                            {
                                client.Dispose();
                                client = null;
                                accessToken = authResponse.Token;
                                return await GetOneAsync<T>(endpoint, true);
                            }
                        }
                        throw new InvalidTokenException();
                    default:
                        result.EnsureSuccessStatusCode();
                        break;
                }
            }

            string contents = await result.Content.ReadAsStringAsync();
            TraceLog?.Invoke(LogLevel.RawData, contents);
            return JsonSerializer.Deserialize<OneResponse<T>>(contents).Response;
        }

        /// <summary>
        /// Post a command with a boolean response.
        /// </summary>
        /// <param name="endpoint">Endpoint</param>
        /// <param name="body">Body of the request</param>
        /// <returns>True/False</returns>
        internal async Task<bool> PostCommandAsync(string endpoint, object body = null)
        {
            return (await PostOneAsync<OneResponse<CommandResponse>>(endpoint, body))
                .Response.Result;
        }

        /// <summary>
        /// POST a command and retrieve a single value from the Tesla API.
        /// </summary>
        /// <param name="endpoint">Endpoint to call</param>
        /// <param name="body">Body of the request</param>
        /// <param name="isRetryWithNewAccessToken">True when this is a retry due to a failed token</param>
        /// <typeparam name="T">Returning type</typeparam>
        /// <returns>Return value from API</returns>
        /// <exception cref="SleepingException">Car is asleep and not accepting commands.</exception>
        internal async Task<T> PostOneAsync<T>(string endpoint, object body = null, bool isRetryWithNewAccessToken = false)
        {
            string bodyText = string.Empty;
            if (body != null)
            {
                bodyText = body is string s ? s : JsonSerializer.Serialize(body);
            }
            
            string url = Constants.VehiclesApi + endpoint;
            TraceLog?.Invoke(LogLevel.Query, $"POST {url}");
            var result = await Client.PostAsync(url, new StringContent(bodyText));
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
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.Unauthorized:
                        if (!isRetryWithNewAccessToken && AutoRefreshToken != null)
                        {
                            var authResponse = AutoRefreshToken.Invoke();
                            if (authResponse?.Token != null)
                            {
                                client.Dispose();
                                client = null;
                                accessToken = authResponse.Token;
                                return await PostOneAsync<T>(endpoint, body, true);
                            }
                        }
                        throw new InvalidTokenException();
                    default:
                        result.EnsureSuccessStatusCode();
                        break;
                }
            }

            string contents = await result.Content.ReadAsStringAsync();
            TraceLog?.Invoke(LogLevel.RawData, contents);
            return JsonSerializer.Deserialize<OneResponse<T>>(contents).Response;
        }

        /// <summary>
        /// Retrieve a set of values from the Tesla API.
        /// </summary>
        /// <param name="endpoint">Endpoint to call</param>
        /// <param name="isRetryWithNewAccessToken">True if this is a retry due to a failed access token</param>
        /// <typeparam name="T">Returning type</typeparam>
        /// <returns>Return value from API</returns>
        /// <exception cref="SleepingException">Car is asleep and not accepting commands.</exception>
        internal async Task<IReadOnlyList<T>> GetListAsync<T>(string endpoint, bool isRetryWithNewAccessToken = false)
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
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.Unauthorized:
                        if (!isRetryWithNewAccessToken && AutoRefreshToken != null)
                        {
                            var authResponse = AutoRefreshToken.Invoke();
                            if (authResponse?.Token != null)
                            {
                                client.Dispose();
                                client = null;
                                accessToken = authResponse.Token;
                                return await GetListAsync<T>(endpoint, true);
                            }
                        }
                        throw new InvalidTokenException();
                    default:
                        result.EnsureSuccessStatusCode();
                        break;
                }
            }

            string contents = await result.Content.ReadAsStringAsync();
            TraceLog?.Invoke(LogLevel.RawData, contents);
            return JsonSerializer.Deserialize<ListResponse<T>>(contents).Response;
        }

        /// <summary>
        /// Login to the Tesla API and retrieve an owner API token.
        /// </summary>
        /// <param name="email">Email account tied to Tesla</param>
        /// <param name="password">Password for the Tesla account</param>
        /// <param name="multiFactorAuthResolver">Optional resolver for Multi-Factor auth.</param>
        /// <returns>Access token information</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<AccessToken> LoginAsync(string email, string password, Func<(string passcode, string backupPasscode)> multiFactorAuthResolver = null)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException($"'{nameof(email)}' cannot be null or empty.", nameof(email));
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            if (string.IsNullOrEmpty(ClientId))
                throw new ArgumentException($"'{nameof(ClientId)}' cannot be null or empty.", nameof(ClientId));
            if (string.IsNullOrEmpty(ClientSecret))
                throw new ArgumentException($"'{nameof(ClientSecret)}' cannot be null or empty.", nameof(ClientSecret));

            try
            {
                return await Auth.GetAccessTokenAsync(email, password,
                    ClientId, ClientSecret, TraceLog, multiFactorAuthResolver);
            }
            catch (TeslaAuthenticationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new TeslaAuthenticationException((int) HttpStatusCode.FailedDependency, "Failed to refresh token.", ex);
            }
        }

        /// <summary>
        /// Used to revoke an app token
        /// </summary>
        /// <param name="token">Token to revoke</param>
        /// <exception cref="TeslaAuthenticationException"></exception>
        public Task RevokeTokenAsync(string token)
        {
            try
            {
                return Auth.RevokeTokenAsync(token, TraceLog);
            }
            catch (TeslaAuthenticationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new TeslaAuthenticationException((int) HttpStatusCode.FailedDependency, "Failed to refresh token.", ex);
            }
        }
        
        /// <summary>
        /// Method to refresh the access token from a refresh token.
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>New access token information</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<AccessToken> RefreshLoginAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(ClientId))
                throw new ArgumentException($"'{nameof(ClientId)}' cannot be null or empty.", nameof(ClientId));
            if (string.IsNullOrEmpty(ClientSecret))
                throw new ArgumentException($"'{nameof(ClientSecret)}' cannot be null or empty.", nameof(ClientSecret));

            try
            {
                return await Auth.RefreshTokenAsync(refreshToken, ClientId, ClientSecret, TraceLog);
            }
            catch (TeslaAuthenticationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new TeslaAuthenticationException((int) HttpStatusCode.FailedDependency, "Failed to refresh token.", ex);
            }
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
