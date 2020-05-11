using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest;

namespace DSharp4Webhook.Internal
{
    internal sealed class UpdateResult : RestResult, IUpdateResult
    {
        public IWebhookInfo WebhookInfo { get; }

        public UpdateResult(IWebhookInfo webhookInfo, RestResponse[] responses) : base(responses)
        {
            WebhookInfo = webhookInfo;
        }
    }
}
