using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest.Manipulation;
using DSharp4Webhook.Serialization;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DSharp4Webhook.Internal
{
    internal sealed class UpdateAction : BaseRestAction<IUpdateResult>, IUpdateAction
    {
        public SerializeContext Context { get; }

        public UpdateAction(SerializeContext context, IWebhook webhook) : base(webhook)
        {
            Context = context;
        }

        public override async Task<bool> ExecuteAsync()
        {
            if (IsExecuted)
                throw new InvalidOperationException("The action has already been performed");
            IsExecuted = true;

            var responses = await Webhook.RestProvider.PATCH(Webhook.GetWebhookUrl(), Context, 1);
            var lastResponse = responses[responses.Length - 1];
            WebhookInfo webhookInfo = JsonConvert.DeserializeObject<WebhookInfo>(lastResponse.Content);
            Result = new UpdateResult(webhookInfo, responses);
            SettingRateLimit();
            return BaseRestProvider.GET_ALLOWED_STATUSES.Contains(lastResponse.StatusCode);
        }
    }
}