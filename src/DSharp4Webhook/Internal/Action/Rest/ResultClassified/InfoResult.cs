using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest;

namespace DSharp4Webhook.Internal
{
    internal sealed class InfoResult : RestResult, IInfoResult
    {
        public IWebhookInfo WebhookInfo { get; }

        public InfoResult(IWebhookInfo webhookInfo, RestResponse[] responses) : base(responses)
        {
            WebhookInfo = webhookInfo;
        }
    }
}
