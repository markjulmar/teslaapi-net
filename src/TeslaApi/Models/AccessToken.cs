using System;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// This is the return object from a login or refresh attempt
    /// </summary>
    public sealed class AccessToken
    {
        /// <summary>
        /// Time this token was created
        /// </summary>
        public DateTime CreatedTimeUtc { get; set; }
        /// <summary>
        /// Time this token expires
        /// </summary>
        public DateTime ExpirationTimeUtc { get; set; }
        /// <summary>
        /// The access token to save off
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Refresh token to refresh access.
        /// </summary>
        public string RefreshToken { get; set; }
    }
}