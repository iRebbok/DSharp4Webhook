using Newtonsoft.Json;
using System;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     Basic webhook information.
    /// </summary>
    /// <remarks>
    ///     Used as constant information for webhook integration.
    ///     The implementation is located in <see cref="WebhookInfo"/>.
    /// </remarks>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    public interface IWebhookInfo
    {
        /// <summary>
        ///     The nickname of the webhook that will be displayed in the message.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     When you try to set <see cref="string.Empty"/> or a value greater than 80 characters.
        /// </exception>
        [JsonProperty(PropertyName = "username")]
        string Username { get; set; }

        /// <summary>
        ///     The webhook avatar that will be displayed in the message.
        /// </summary>
        [JsonProperty(PropertyName = "avatar_url")]
        string AvatarUrl { get; set; }
    }
}
