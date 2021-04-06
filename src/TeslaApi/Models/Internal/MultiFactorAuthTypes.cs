using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Julmar.TeslaApi.Internal
{
    /// <summary>
    /// This object is the JSON response to /oauth2/v3/authorize/mfa/factors
    /// and determines which MFA response types are allowed.
    /// </summary>
    sealed class MultiFactorAuthType
    {
        //[JsonPropertyName("dispatchRequired")]
        //public bool DispatchRequired { get; set; }

        /// <summary>
        /// The id assigned by Tesla
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        //[JsonPropertyName("name")]
        //public string Name { get; set; }

        /// <summary>
        /// The MFA type - 'token:software'
        /// </summary>  
        [JsonPropertyName("factorType")]
        public string FactorType { get; set; }

        //[JsonPropertyName("factorProvider")]
        //public string FactorProvider { get; set; }

        //[JsonPropertyName("securityLevel")]
        //public int SecurityLevel { get; set; }

        //[JsonPropertyName("activatedAt")]
        //public DateTime ActivatedAt { get; set; }

        //[JsonPropertyName("updatedAt")]
        //public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Response JSON object from the MFA verify endpoint - oauth2/v3/authorize/mfa/verify
    /// when the passcode approach is utilized.
    /// </summary>
    sealed class MultiFactorPasscodeAuthValidation
    {
        /// <summary>
        /// MFA identifier
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// Challenge id
        /// </summary>
        [JsonPropertyName("challengeId")]
        public string ChallengeId { get; set; }

        /// <summary>
        /// MFA factor id passed in
        /// </summary>
        [JsonPropertyName("factorId")]
        public string FactorId { get; set; }

        /// <summary>
        /// Passcode passed in
        /// </summary>
        [JsonPropertyName("passCode")]
        public string PassCode { get; set; }

        /// <summary>
        /// True if MFA was approved
        /// </summary>
        [JsonPropertyName("approved")]
        public bool Approved { get; set; }

        /// <summary>
        /// True if MFA was flagged by the system
        /// </summary>
        [JsonPropertyName("flagged")]
        public bool Flagged { get; set; }

        /// <summary>
        /// True if MFA is valid.
        /// </summary>
        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        /// <summary>
        /// Date MFA validation was created
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date MFA validation was updated (tends to be same as created).
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Response JSON object from the MFA verify endpoint - oauth2/v3/authorize/mfa/verify
    /// when the backup passcode approach is utilized.
    /// </summary>
    class MultiFactorBackupCodeAuthValidation
    {
        /// <summary>
        /// True if the MFA validation was succesful.
        /// </summary>
        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        /// <summary>
        /// Reason (null)
        /// </summary>
        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        /// <summary>
        /// Message (null)
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// True if user enrolled in MFA
        /// </summary>
        [JsonPropertyName("enrolled")]
        public bool Enrolled { get; set; }

        /// <summary>
        /// Date code was generated on
        /// </summary>
        [JsonPropertyName("generatedAt")]
        public DateTime GeneratedAt { get; set; }

        /// <summary>
        /// # of remaining backup codes
        /// </summary>
        [JsonPropertyName("codesRemaining")]
        public int CodesRemaining { get; set; }

        /// <summary>
        /// # of valid attempts remaining on code
        /// </summary>
        [JsonPropertyName("attemptsRemaining")]
        public int AttemptsRemaining { get; set; }

        /// <summary>
        /// True if backup code is locked.
        /// </summary>
        [JsonPropertyName("locked")]
        public bool Locked { get; set; }
    }

    /// <summary>
    /// Wrapper for the authentication factor list.
    /// </summary>
    sealed class AuthTypeWrapper
    {
        [JsonPropertyName("data")]
        public List<MultiFactorAuthType> Data { get; set; }
    }

    /// <summary>
    /// Wrapper for the passcode/backup passcode response
    /// </summary>
    /// <typeparam name="TData">Specific auth type</typeparam>
    sealed class AuthWrapper<TData>
    {
        [JsonPropertyName("data")]
        public TData Data { get; set; }
    }
    
    /// <summary>
    /// This JSON object is returned from all of the OAuth endpoints
    /// and includes the token, type and refresh information.
    /// </summary>
    sealed class BearerTokenResponse
    {
        /// <summary>
        /// Bearer access token returned from API
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh token returned from API
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Seconds the access token is valid for,
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpiresInSeconds { get; set; }
        
        /// <summary>
        /// Date/Time the token was created at
        /// </summary>

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }

        /// <summary>
        /// The token type (bearer)
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}
