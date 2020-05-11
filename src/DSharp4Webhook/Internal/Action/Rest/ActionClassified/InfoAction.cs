using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest.Manipulation;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace DSharp4Webhook.Internal
{
    internal sealed class InfoAction : BaseRestAction<IInfoResult>, IInfoAction
    {
        public InfoAction(IWebhook webhook) : base(webhook) { }

        public override async Task<bool> ExecuteAsync()
        {
            var responses = await Webhook.RestProvider.GET(Webhook.GetWebhookUrl(), 1);
            var lastResponse = responses[responses.Length - 1];
            WebhookInfo webhookInfo = JsonConvert.DeserializeObject<WebhookInfo>(lastResponse.Content);
            Result = new InfoResult(webhookInfo, responses);
            SettingRateLimit();
            return BaseRestProvider.GET_ALLOWED_STATUSES.Contains(lastResponse.StatusCode);
        }
    }
}
