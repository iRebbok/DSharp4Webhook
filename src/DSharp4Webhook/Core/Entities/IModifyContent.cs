using DSharp4Webhook.Serialization;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     Data that modifies the webhook.
    /// </summary>
    public interface IModifyContent : IWSerializable
    {
        /// <summary>
        ///     Webhook name.
        /// </summary>
#nullable enable
        public string? Name { get; }
#nullable restore

        /// <summary>
        ///     Webhook image.
        /// </summary>
#nullable enable
        public IWebhookImage? Image { get; }
#nullable restore
    }
}
