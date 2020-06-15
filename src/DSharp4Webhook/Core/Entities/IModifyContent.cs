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
        public string Name { get; }

        /// <summary>
        ///     Webhook image.
        /// </summary>
        public IWebhookImage? Image { get; }
    }
}
