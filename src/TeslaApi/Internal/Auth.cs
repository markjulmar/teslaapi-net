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

namespace TeslaApi.Internal
{
    static class Auth
    {
        public static async Task<(string accessToken, string refreshToken)> GetAccessTokenAsync(
            string user, string password,
            string clientId, string clientSecret,
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

            // Will return 'tesla-auth.sid' as cookie.
            HttpClientHandler handler = new() { AllowAutoRedirect = false, CookieContainer = new CookieContainer() };

            // Use curl as agent
            using var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "curl / 6.14.0");
            var htmlResponse = await client.GetAsync(uri);

            htmlResponse.EnsureSuccessStatusCode();

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
            var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = new FormUrlEncodedContent(formValues) };
            htmlResponse = await client.SendAsync(request);

            htmlResponse.EnsureSuccessStatusCode();
            htmlForm = await htmlResponse.Content.ReadAsStringAsync();

            // Deal with MFA
            bool usesMultiFactor = htmlForm.Contains("/mfa/verify");
            if (usesMultiFactor && multiFactorAuthResolver != null)
            {
                // Get either a passcode or backup
                (string passcode, string backupPasscode) = multiFactorAuthResolver();

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
                    string formTypes = await client.GetStringAsync(Constants.MfaTypesUrl + "?transaction_id=" + transactionId);
                    var mfaType = JsonSerializer.Deserialize<AuthTypeWrapper>(formTypes).Data.SingleOrDefault(at => at.FactorType == "token:software");
                    if (mfaType != null)
                    {
                        // data = {"transaction_id": transaction_id, "factor_id": factor_id, "passcode": args.passcode}
                        var mfaValidationForm = JsonSerializer.Serialize(new Dictionary<string, string> {
                            { "transaction_id", transactionId },
                            { "factor_id", mfaType.Id },
                            { "passcode", passcode }
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
                        JsonSerializer.Serialize(new Dictionary<string, string>() { { "transaction_id", transactionId }, { "backup_code", backupPasscode } }),
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
                    throw new Exception("Account has multi-factor authentication enabled.");

                htmlResponse = await client.PostAsync(Constants.MfaVerify, authBody);
                htmlResponse.EnsureSuccessStatusCode();
                htmlForm = await htmlResponse.Content.ReadAsStringAsync();

                Console.WriteLine(htmlForm);
                Console.WriteLine();

                if (!htmlForm.Contains("error"))
                {
                    if (passcode != null)
                    {
                        var authValid = JsonSerializer.Deserialize<AuthWrapper<MultiFactorPasscodeAuthValidation>>(htmlForm).Data;
                        if (!authValid.Valid || authValid.Flagged || !authValid.Approved)
                            throw new Exception("Passcode could not be validated.");
                    }
                    else if (backupPasscode != null)
                    {
                        var authValid = JsonSerializer.Deserialize<AuthWrapper<MultiFactorBackupCodeAuthValidation>>(htmlForm).Data;
                        if (!authValid.Valid)
                            throw new Exception("Backup code could not be validated.");
                    }
                }
                else throw new Exception($"Multi-factor authentication failed: {htmlForm}");
            }

            // Send request again, should get redirect (302) with location.
            htmlResponse = await client.PostAsync(uri, new StringContent(JsonSerializer.Serialize(new Dictionary<string, string>() { { "transaction_id", transactionId } }),
                                                    Encoding.UTF8, "application/json"));
            // TODO: need retry logic?
            string authToken = null;
            if (htmlResponse.StatusCode == HttpStatusCode.Redirect)
            {
                var location = htmlResponse.Headers.Location;
                authToken = HttpUtility.ParseQueryString(location.Query).Get("code");
            }

            if (authToken == null)
                throw new Exception("Unable to get authorization token.");

            Console.WriteLine($"Authorization Token: {authToken}");

            // Exchange the auth token for a bearer token.
            htmlResponse = await client.PostAsync(Constants.TokenExchangeUrl, new StringContent(JsonSerializer.Serialize(
                new Dictionary<string, string>()
                {
                    { "grant_type", "authorization_code" },
                    { "client_id", "ownerapi" },
                    { "code", authToken },
                    { "code_verifier", codeVerifierText },
                    { "redirect_uri", Constants.RedirectUri }
                }), Encoding.UTF8, "application/json"));

            string bearerTokenText = await htmlResponse.Content.ReadAsStringAsync();
            Console.WriteLine(bearerTokenText);

            //{
            //  "access_token": "xyx",
            //  "refresh_token": "xyz",
            //  "expires_in": 300,
            //  "state": "of the union",
            //  "token_type": "Bearer"
            //}

            var bearerTokenResponse = JsonSerializer.Deserialize<BearerTokenResponse>(bearerTokenText);
            if (bearerTokenResponse.TokenType != "Bearer")
                throw new Exception("Failed to exchange the authentication token for a bearer token.");

            Console.WriteLine($"Bearer Token: {bearerTokenResponse.AccessToken}");
            Console.WriteLine($"Refresh Token: {bearerTokenResponse.RefreshToken}");

            // This is the one to save. The owner API one below is a dummy value.
            string refreshToken = bearerTokenResponse.RefreshToken;

            // Finally, exchange the bearer token for a long-lived access token.
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerTokenResponse.AccessToken);
            htmlResponse = await client.PostAsync(Constants.OwnerApiTokenUrl, new StringContent(JsonSerializer.Serialize(
                new Dictionary<string, string>()
                {
                    { "grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer" },
                    { "client_id", clientId },
                    { "client_secret", clientSecret }
                }), Encoding.UTF8, "application/json"));

            bearerTokenText = await htmlResponse.Content.ReadAsStringAsync();
            Console.WriteLine(bearerTokenText);

            bearerTokenResponse = JsonSerializer.Deserialize<BearerTokenResponse>(bearerTokenText);
            if (bearerTokenResponse.TokenType != "bearer")
                throw new Exception("Failed to exchange the bearer token for an access token.");

            Console.WriteLine($"Access Token: {bearerTokenResponse.AccessToken}");

            return (bearerTokenResponse.AccessToken, refreshToken);
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
