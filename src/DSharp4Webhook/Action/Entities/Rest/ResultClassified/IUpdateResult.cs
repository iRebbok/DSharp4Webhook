using DSharp4Webhook.Core;

namespace DSharp4Webhook.Action.Rest
{
    public interface IUpdateResult : IRestResult
    {
        /// <summary>
        ///     Returned updated information about webhook.
        /// </summary>
        public IWebhookInfo WebhookInfo { get; }
    }
}
