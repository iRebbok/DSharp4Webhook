using DSharp4Webhook.Core;

namespace DSharp4Webhook.Actions.Rest
{
    public interface IInfoResult : IRestResult
    {
        /// <summary>
        ///     Returns information about webhook.
        /// </summary>
        public IWebhookInfo WebhookInfo { get; }
    }
}
