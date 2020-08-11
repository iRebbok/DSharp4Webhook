using DSharp4Webhook.Core;

namespace DSharp4Webhook.Actions.Rest
{
    public interface IAvatarResult : IRestResult
    {
        /// <summary>
        ///     Webhook image.
        /// </summary>
        public IWebhookImage Image { get; }
    }
}
