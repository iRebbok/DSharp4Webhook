using System;

using Newtonsoft.Json;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     The webhook message.
    /// </summary>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    public interface IWebhookMessage : IWebhookMessageInfo
    {
        /// <summary>
        ///     The content of the message.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     When you try to set a value greater than 2000 characters.
        /// </exception>
        [JsonProperty(PropertyName = "content")]
        string Content { get; set; }

        /// <summary>
        ///     Whether the TTS determines this message or not.
        /// </summary>
        [JsonProperty(PropertyName = "tts")]
        bool IsTTS { get; set; }
    }
}
