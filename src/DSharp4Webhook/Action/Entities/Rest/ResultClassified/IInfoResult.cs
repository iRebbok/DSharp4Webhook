using DSharp4Webhook.Core;

namespace DSharp4Webhook.Action.Rest
{
    public interface IInfoResult : IRestResult
    {
        /// <summary>
        ///     Returns information about webhook.
        /// </summary>
        public IWebhookInfo WebhookInfo { get; }
    }
}
