using DSharp4Webhook.Actions.Rest;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest;

namespace DSharp4Webhook.Internal
{
    internal sealed class ModifyResult : RestResult, IModifyResult
    {
        public IWebhookInfo WebhookInfo { get; }

        public ModifyResult(IWebhookInfo webhookInfo, RestResponse[] responses) : base(responses)
        {
            WebhookInfo = webhookInfo;
        }
    }
}
