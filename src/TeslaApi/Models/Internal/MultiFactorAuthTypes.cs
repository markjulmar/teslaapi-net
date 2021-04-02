using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Julmar.TeslaApi.Internal
{
    class MultiFactorAuthType
    {
        [JsonPropertyName("dispatchRequired")]
        public bool DispatchRequired { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("factorType")]
        public string FactorType { get; set; }

        [JsonPropertyName("factorProvider")]
        public string FactorProvider { get; set; }

        [JsonPropertyName("securityLevel")]
        public int SecurityLevel { get; set; }

        [JsonPropertyName("activatedAt")]
        public DateTime ActivatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    class MultiFactorPasscodeAuthValidation
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("challengeId")]
        public string ChallengeId { get; set; }

        [JsonPropertyName("factorId")]
        public string FactorId { get; set; }

        [JsonPropertyName("passCode")]
        public string PassCode { get; set; }

        [JsonPropertyName("approved")]
        public bool Approved { get; set; }

        [JsonPropertyName("flagged")]
        public bool Flagged { get; set; }

        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    class MultiFactorBackupCodeAuthValidation
    {
        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("enrolled")]
        public bool Enrolled { get; set; }

        [JsonPropertyName("generatedAt")]
        public DateTime GeneratedAt { get; set; }

        [JsonPropertyName("codesRemaining")]
        public int CodesRemaining { get; set; }

        [JsonPropertyName("attemptsRemaining")]
        public int AttemptsRemaining { get; set; }

        [JsonPropertyName("locked")]
        public bool Locked { get; set; }
    }

    class BearerTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresInMinutes { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }

    class AuthTypeWrapper
    {
        [JsonPropertyName("data")]
        public List<MultiFactorAuthType> Data { get; set; }
    }

    class AuthWrapper<TData>
    {
        [JsonPropertyName("data")]
        public TData Data { get; set; }
    }
}
