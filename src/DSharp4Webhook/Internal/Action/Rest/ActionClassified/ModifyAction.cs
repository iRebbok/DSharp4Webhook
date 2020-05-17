using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest.Manipulation;
using DSharp4Webhook.Serialization;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace DSharp4Webhook.Internal
{
    internal sealed class ModifyAction : BaseRestAction<IModifyResult>, IModifyAction
    {
        public SerializeContext Context { get; }

        public ModifyAction(SerializeContext context, IWebhook webhook) : base(webhook)
        {
            Context = context;
        }

        public override async Task<bool> ExecuteAsync()
        {
            CheckExecution();
            var responses = await Webhook.RestProvider.PATCH(Webhook.GetWebhookUrl(), Context, 1);
            var lastResponse = responses[responses.Length - 1];
            WebhookInfo webhookInfo = JsonConvert.DeserializeObject<WebhookInfo>(lastResponse.Content);
            webhookInfo._webhook = Webhook;
            Result = new ModifyResult(webhookInfo, responses);
            SettingRateLimit();
            return BaseRestProvider.GET_ALLOWED_STATUSES.Contains(lastResponse.StatusCode);
        }
    }
}
