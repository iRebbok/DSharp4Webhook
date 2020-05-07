using System;

using Newtonsoft.Json;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     Basic webhook information.
    /// </summary>
    /// <remarks>
    ///     Used as constant information for webhook integration.
    ///     The implementation is located in <see cref="WebhookMessageInfo"/>.
    /// </remarks>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    public interface IWebhookMessageInfo
    {
        /// <summary>
        ///     The nickname of the webhook that will be displayed in the message.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     When you try to set <see cref="string.Empty"/> or a value greater than 80 characters.
        /// </exception>
#nullable enable
        [JsonProperty(PropertyName = "username")]
        string Username { get; set; }

        /// <summary>
        ///     The webhook avatar that will be displayed in the message.
        /// </summary>
#nullable enable
        [JsonProperty(PropertyName = "avatar_url")]
        string AvatarUrl { get; set; }
    }
}
