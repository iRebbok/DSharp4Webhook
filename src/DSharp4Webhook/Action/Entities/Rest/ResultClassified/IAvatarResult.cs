using DSharp4Webhook.Core;

namespace DSharp4Webhook.Action.Rest
{
    public interface IAvatarResult : IRestResult
    {
        /// <summary>
        ///     Webhook image.
        /// </summary>
        public IWebhookImage Image { get; }
    }
}
