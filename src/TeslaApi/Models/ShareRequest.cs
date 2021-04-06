using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Julmar.TeslaApi
{
    /// <summary>
    /// JSON model for a Share request.
    /// </summary>
    public sealed class ShareRequest
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">URL or address to share</param>
        /// <param name="locale">Locale</param>
        public ShareRequest(string value, string locale = null)
        {
            Type = "share_ext_content_raw";
            Value.Url = value;
            Locale = locale ?? CultureInfo.CurrentUICulture.Name;
            if (string.IsNullOrEmpty(Locale))
                Locale = "en-US";
            UnixTimestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0))
                .TotalSeconds.ToString(CultureInfo.InvariantCulture);
        }
        
        /// <summary>
        /// Type - must be 'share_ext_content_raw'
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Address or video URL to share
        /// </summary>
        [JsonPropertyName("value")]
        public UrlValue Value { get; set; }

        /// <summary>
        /// The locale for the navigation request. ISO 639-1 standard language codes.
        /// </summary>
        [JsonPropertyName("locale")]
        public string Locale { get; set; }

        /// <summary>
        /// The current UNIX timestamp.
        /// </summary>
        [JsonPropertyName("timestamp_ms")]
        public string UnixTimestamp { get; set; }
        
        /// <summary>
        /// The URL to share with the vehicle.
        /// </summary>
        public class UrlValue
        {
            /// <summary>
            /// URL to share
            /// </summary>
            [JsonPropertyName("android.intent.extra.TEXT")]
            public string Url { get; set; }
        }
    }
}