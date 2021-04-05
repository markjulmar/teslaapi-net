using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace Julmar.TeslaApi.Internal
{
    /// <summary>
    /// Class to manage authentication to the Tesla API.
    /// This supports both standard and multi-factor auth.
    /// </summary>
    internal static class Auth
    {
        /// <summary>
        /// Retrieves an owner access token from a user/password.
        /// </summary>
        /// <param name="user">Email for Telsa account</param>
        /// <param name="password">Password for account</param>
        /// <param name="clientId">OAuth exchange client Id</param>
        /// <param name="clientSecret">OAuth exchange client Secret</param>
        /// <param name="logger">Optional logger</param>
        /// <param name="multiFactorAuthResolver">MFA resolver to retrieve passcode or backup code.</param>
        /// <returns>access token and refresh token</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        internal static async Task<AccessToken> GetAccessTokenAsync(
            string user, string password,
            string clientId, string clientSecret,
            Action<LogLevel, string> logger,
            Func<(string passcode, string backupPasscode)> multiFactorAuthResolver = null)
        {
            if (string.IsNullOrEmpty(user))
                throw new ArgumentException($"'{nameof(user)}' cannot be null or empty.", nameof(user));

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));

            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException($"'{nameof(clientId)}' cannot be null or empty.", nameof(clientId));

            if (string.IsNullOrEmpty(clientSecret))
                throw new ArgumentException($"'{nameof(clientSecret)}' cannot be null or empty.", nameof(clientSecret));

            string codeVerifierText = GetRandomAlphaText(86);
            string codeChallenge = CreateHash(codeVerifierText);
            string state = GetRandomAlphaText(20);
            var uri = BuildAuthUri(codeChallenge, state);

            logger?.Invoke(LogLevel.Info, "Step 1: Obtain the login page.");
            
            var client = BuildHttpClient();
            logger?.Invoke(LogLevel.Query, $"GET {uri}");
            var htmlResponse = await client.GetAsync(uri);
            logger?.Invoke(LogLevel.Response, htmlResponse.ToString());

            if (!htmlResponse.IsSuccessStatusCode)
                throw new TeslaAuthenticationException((int) htmlResponse.StatusCode, htmlResponse.ReasonPhrase);

            // Grab the form and all the hidden input fields.
            var htmlForm = await htmlResponse.Content.ReadAsStringAsync();

            HtmlNode.ElementsFlags.Remove("form");
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlForm);

            var formValues = htmlDoc.DocumentNode.SelectNodes("//form/input")
                .Where(n => !string.IsNullOrEmpty(n.GetAttributeValue("name", "")))
                .ToDictionary(n => n.GetAttributeValue("name", ""), n => n.GetAttributeValue("value", ""));

            string transactionId = formValues["transaction_id"];

            // Add username/password
            formValues.Add("identity", user);
            formValues.Add("credential", password);

            // Send the login
            logger?.Invoke(LogLevel.Info, "Step 2: Obtain an authorization code.");
            logger?.Invoke(LogLevel.Query, $"POST {uri}");
            var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = new FormUrlEncodedContent(formValues) };
            htmlResponse = await client.SendAsync(request);
            logger?.Invoke(LogLevel.Response, htmlResponse.ToString());

            // If we get a form back, then the Tesla site is asking for more info.
            if (htmlResponse.StatusCode == HttpStatusCode.OK)
            {
                htmlForm = await htmlResponse.Content.ReadAsStringAsync();

                // Deal with MFA
                bool usesMultiFactor = htmlForm.Contains("/mfa/verify");
                if (usesMultiFactor && multiFactorAuthResolver != null)
                {
                    logger?.Invoke(LogLevel.Info, "Step 2a: Multi-factor authentication is enabled. Retrieving passcode and attempting login.");
                    
                    // Get either a passcode or backup
                    var (passcode, backupPasscode) = multiFactorAuthResolver();

                    HttpContent authBody = null;
                    if (passcode != null)
                    {
                        // Need a passcode or backup code.
                        //{
                        //  "data": [
                        //    {
                        //      "dispatchRequired": false,
                        //      "id": "978676c5-70c0-44e1-8cc8-353d787eba05",
                        //      "name": "Device #1",
                        //      "factorType": "token:software",
                        //      "factorProvider": "TESLA",
                        //      "securityLevel": 1,
                        //      "activatedAt": "2021-03-22T16:00:13.000Z",
                        //      "updatedAt": "2021-03-22T16:00:13.000Z"
                        //    }
                        //  ]
                        //}
                        logger?.Invoke(LogLevel.Query, $"GET {Constants.MfaTypesUrl}");
                        string formTypes = await client.GetStringAsync(Constants.MfaTypesUrl + "?transaction_id=" + transactionId);
                        logger?.Invoke(LogLevel.RawData, formTypes);
                        var mfaType = JsonSerializer.Deserialize<AuthTypeWrapper>(formTypes).Data
                                            .SingleOrDefault(at => at.FactorType == "token:software");
                        if (mfaType != null)
                        {
                            // data = {"transaction_id": transaction_id, "factor_id": factor_id, "passcode": args.passcode}
                            var mfaValidationForm = JsonSerializer.Serialize(new Dictionary<string, string>
                            {
                                {"transaction_id", transactionId},
                                {"factor_id", mfaType.Id},
                                {"passcode", passcode}
                            });
                            authBody = new StringContent(mfaValidationForm, Encoding.UTF8, "application/json");
                            // Response
                            //{
                            //  "data": {
                            //    "id": "24d3d320-932f-11eb-9d40-453692733d72",
                            //    "challengeId": "247cb270-932f-11eb-9d40-453692733d72",
                            //    "factorId": "978676c5-70c0-44e1-8cc8-353d787eba05",
                            //    "passCode": "500246",
                            //    "approved": true,
                            //    "flagged": false,
                            //    "valid": true,
                            //    "createdAt": "2021-04-01T14:13:45.000Z",
                            //    "updatedAt": "2021-04-01T14:13:45.000Z"
                            //  }
                            //}
                        }
                    }
                    else if (backupPasscode != null)
                    {
                        // data = {"transaction_id": transaction_id, "backup_code": args.backup_passcode}
                        authBody = new StringContent(
                            JsonSerializer.Serialize(new Dictionary<string, string>()
                                {{"transaction_id", transactionId}, {"backup_code", backupPasscode}}),
                            Encoding.UTF8, "application/json");
                        // Response
                        //{
                        //    "data": {
                        //        "valid": true,
                        //        "reason": null,
                        //        "message": null,
                        //        "enrolled": true,
                        //        "generatedAt": "2021-04-01T14:13:45.000Z",
                        //        "codesRemaining": 9,
                        //        "attemptsRemaining": 10,
                        //        "locked": false,
                        //    }
                        //}
                    }

                    if (authBody == null)
                        throw new TeslaAuthenticationException((int) HttpStatusCode.Unauthorized,
                            "Account has multi-factor authentication enabled. You must supply a passcode or backup passcode.");

                    logger?.Invoke(LogLevel.Query, $"POST {Constants.MfaVerify}");
                    htmlResponse = await client.PostAsync(Constants.MfaVerify, authBody);
                    logger?.Invoke(LogLevel.Response, htmlResponse.ToString());
                    
                    if (!htmlResponse.IsSuccessStatusCode)
                        throw new TeslaAuthenticationException((int) htmlResponse.StatusCode, htmlResponse.ReasonPhrase);

                    htmlForm = await htmlResponse.Content.ReadAsStringAsync();
                    logger?.Invoke(LogLevel.RawData, htmlForm);

                    if (!htmlForm.Contains("error"))
                    {
                        if (passcode != null)
                        {
                            var authValid = JsonSerializer.Deserialize<AuthWrapper<MultiFactorPasscodeAuthValidation>>(htmlForm).Data;
                            if (!authValid.Valid || authValid.Flagged || !authValid.Approved)
                                throw new TeslaAuthenticationException((int) HttpStatusCode.Unauthorized, 
                                    "MFA Passcode could not be validated.");
                        }
                        else
                        {
                            var authValid = JsonSerializer.Deserialize<AuthWrapper<MultiFactorBackupCodeAuthValidation>>(htmlForm).Data;
                            if (!authValid.Valid || authValid.Locked)
                                throw new TeslaAuthenticationException((int) HttpStatusCode.Unauthorized, "Backup code could not be validated.");
                        }
                    }
                    else throw new TeslaAuthenticationException((int) HttpStatusCode.Unauthorized, $"Multi-factor authentication failed: {htmlForm}");
                }

                // Send request again, should get redirect (302) with location.
                logger?.Invoke(LogLevel.Query, $"POST {uri}");
                htmlResponse = await client.PostAsync(uri, new StringContent(
                    JsonSerializer.Serialize(new Dictionary<string, string>() {{"transaction_id", transactionId}}),
                    Encoding.UTF8, "application/json"));
                logger?.Invoke(LogLevel.Response, htmlResponse.ToString());
            }

            // We should have a 302 redirect at this point.
            if (htmlResponse.StatusCode != HttpStatusCode.Redirect)
                throw new TeslaAuthenticationException((int) HttpStatusCode.BadRequest, "Unexpected authentication flow, could not sign in.");

            // Get the location - it's fake but has the code.
            var location = htmlResponse.Headers.Location;
            var authToken = HttpUtility.ParseQueryString(location.Query).Get("code");
            if (authToken == null)
                throw new TeslaAuthenticationException((int) HttpStatusCode.BadRequest, "Unable to get authorization token from Tesla login.");

            logger?.Invoke(LogLevel.Info, $"Step 3: Exchange authorization code for bearer token.");

            // Exchange the auth token for a bearer token.
            logger?.Invoke(LogLevel.Query, $"POST {Constants.TokenExchangeUrl}");
            htmlResponse = await client.PostAsync(Constants.TokenExchangeUrl, new StringContent(JsonSerializer.Serialize(
                new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "client_id", "ownerapi" },
                    { "code", authToken },
                    { "code_verifier", codeVerifierText },
                    { "redirect_uri", Constants.RedirectUri }
                }), Encoding.UTF8, "application/json"));
            logger?.Invoke(LogLevel.Response, htmlResponse.ToString());

            string bearerTokenText = await htmlResponse.Content.ReadAsStringAsync();
            logger?.Invoke(LogLevel.RawData, bearerTokenText);

            //{
            //  "access_token": "xyx",
            //  "refresh_token": "xyz",
            //  "expires_in": 300,
            //  "state": "of the union",
            //  "token_type": "Bearer"
            //}

            var bearerTokenResponse = JsonSerializer.Deserialize<BearerTokenResponse>(bearerTokenText);
            if (bearerTokenResponse?.TokenType != "Bearer")
                throw new TeslaAuthenticationException((int) HttpStatusCode.BadRequest, "Failed to exchange the authentication token for a bearer token.");

            // This is the one to save. The owner API one below is a dummy value.
            var tokenResponse = new AccessToken
            {
                RefreshToken = bearerTokenResponse.RefreshToken
            };

            logger?.Invoke(LogLevel.Info, $"Step 4: exchange bearer token for owner API auth token.");
            bearerTokenResponse = await ExchangeTokenAsync(client, clientId, clientSecret, bearerTokenResponse.AccessToken, logger);

            tokenResponse.CreatedTimeUtc = DateTimeOffset.FromUnixTimeSeconds(bearerTokenResponse.CreatedAt).UtcDateTime;
            tokenResponse.ExpirationTimeUtc = DateTime.UtcNow.AddSeconds(bearerTokenResponse.ExpiresInSeconds);
            tokenResponse.Token = bearerTokenResponse.AccessToken;

            return tokenResponse;
        }

        private static async Task<BearerTokenResponse> ExchangeTokenAsync(HttpClient client, string clientId, string clientSecret,
            string accessToken, Action<LogLevel, string> logger)
        {
            // Finally, exchange the bearer token for a long-lived access token.
            logger?.Invoke(LogLevel.Query, $"POST {Constants.OwnerApiTokenUrl}");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
            var htmlResponse = await client.PostAsync(Constants.OwnerApiTokenUrl, new StringContent(JsonSerializer.Serialize(
                new Dictionary<string, string>
                {
                    {"grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"},
                    {"client_id", clientId},
                    {"client_secret", clientSecret}
                }), Encoding.UTF8, "application/json"));
            logger?.Invoke(LogLevel.Response, htmlResponse.ToString());

            var bearerTokenText = await htmlResponse.Content.ReadAsStringAsync();
            logger?.Invoke(LogLevel.RawData, bearerTokenText);

            var accessTokenResponse = JsonSerializer.Deserialize<BearerTokenResponse>(bearerTokenText);
            if (accessTokenResponse?.TokenType != "bearer")
                throw new TeslaAuthenticationException((int) HttpStatusCode.BadRequest, "Failed to exchange the bearer token for an owner access token.");

            return accessTokenResponse;
        }

        /// <summary>
        /// Revokes the supplied access token issued by the token command.
        /// </summary>
        /// <param name="accessToken">Access token to revoke</param>
        /// <param name="logger">Optional logger</param>
        internal static async Task RevokeTokenAsync(string accessToken, Action<LogLevel, string> logger = null)
        {
            logger?.Invoke(LogLevel.Query, $"POST {Constants.OwnerApiTokenRevokeUrl}");
            var client = BuildHttpClient();
            var htmlResponse = await client.PostAsync(Constants.OwnerApiTokenRevokeUrl, new StringContent(JsonSerializer.Serialize(
                new Dictionary<string, string> {{ "token", accessToken }}), Encoding.UTF8, "application/json"));
            logger?.Invoke(LogLevel.Response, htmlResponse.ToString());
        }

        /// <summary>
        /// Create a new http client.
        /// </summary>
        private static HttpClient BuildHttpClient()
        {
            var client =new HttpClient(new HttpClientHandler { AllowAutoRedirect = false, CookieContainer = new CookieContainer() });
            client.DefaultRequestHeaders.Add("User-Agent", "curl / 6.14.0");
            return client;
        }
        
        /// <summary>
        /// Refresh the access token.
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="clientId">OAuth exchange client Id</param>
        /// <param name="clientSecret">OAuth exchange client Secret</param>
        /// <param name="logger"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static async Task<AccessToken> RefreshTokenAsync(string refreshToken,
            string clientId, string clientSecret, Action<LogLevel, string> logger = null)
        {
            logger?.Invoke(LogLevel.Query, $"POST {Constants.TokenExchangeUrl}");

            var client = BuildHttpClient();
            var htmlResponse = await client.PostAsync(Constants.TokenExchangeUrl, new StringContent(JsonSerializer.Serialize(
                new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "client_id", "ownerapi" },
                    { "refresh_token", refreshToken },
                    { "scope", "openid email offline_access" }
                }), Encoding.UTF8, "application/json"));
            logger?.Invoke(LogLevel.Response, htmlResponse.ToString());

            var bearerTokenText = await htmlResponse.Content.ReadAsStringAsync();
            logger?.Invoke(LogLevel.RawData, bearerTokenText);

            //{
            //  "access_token": "xyx",
            //  "refresh_token": "xyz",
            //  "expires_in": 300,
            //  "id_token": "id",
            //  "token_type": "Bearer"
            //}

            var bearerTokenResponse = JsonSerializer.Deserialize<BearerTokenResponse>(bearerTokenText);
            if (bearerTokenResponse?.TokenType != "Bearer")
                throw new Exception("Failed to exchange the bearer token for an owner access token.");

            // This is the one to save. The owner API one below is a dummy value.
            var tokenResponse = new AccessToken
            {
                RefreshToken = bearerTokenResponse.RefreshToken
            };

            logger?.Invoke(LogLevel.Info, $"Refresh: exchange bearer token for owner API auth token.");
            bearerTokenResponse = await ExchangeTokenAsync(client, clientId, clientSecret, bearerTokenResponse.AccessToken, logger);

            tokenResponse.CreatedTimeUtc = DateTimeOffset.FromUnixTimeSeconds(bearerTokenResponse.CreatedAt).UtcDateTime;
            tokenResponse.ExpirationTimeUtc = DateTime.UtcNow.AddSeconds(bearerTokenResponse.ExpiresInSeconds);
            tokenResponse.Token = bearerTokenResponse.AccessToken;
            return tokenResponse;
        }
        
        private static Uri BuildAuthUri(string challenge, string state)
        {
            var builder = new UriBuilder(Constants.AuthUrl);
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["client_id"] = "ownerapi";
            parameters["code_challenge"] = challenge;
            parameters["code_challenge_method"] = "S256";
            parameters["redirect_uri"] = Constants.RedirectUri;
            parameters["response_type"] = "code";
            parameters["scope"] = "openid email offline_access";
            parameters["state"] = state;
            builder.Query = parameters.ToString();
            return builder.Uri;
        }

        private static string CreateHash(string value)
        {
            // SHA-256 hash of value
            using var hash = SHA256.Create();
            string hashValue = string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(value))
                                        .Select(item => item.ToString("x2")));
            // Base64 encoded
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(hashValue)).Trim('=');
        }

        private static string GetRandomAlphaText(int count)
        {
            const string alphanumericCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var rng = new Random();
            return new string(Enumerable.Range(0, count)
                    .Select(_ => alphanumericCharacters[rng.Next(alphanumericCharacters.Length)])
                    .ToArray());
        }
    }
}
