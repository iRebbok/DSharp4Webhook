using DSharp4Webhook.Action;
using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest.Manipulation;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DSharp4Webhook.Internal
{
    internal sealed class MessageAction : BaseRestAction<IRestResult>, IMessageAction
    {
        public IMessage Message { get; }

        public MessageAction(IMessage message, IWebhook webhook) : base(webhook)
        {
            Message = message;
        }

        public override async Task<bool> ExecuteAsync()
        {
            if (IsExecuted)
                throw new InvalidOperationException("The action has already been performed");
            IsExecuted = true;

            Result = new RestResult(await Webhook.RestProvider.POST(Webhook.GetWebhookUrl(), Message.Serialize(), 1));
            SettingRateLimit();
            return Result.LastResponse.HasValue && BaseRestProvider.POST_ALLOWED_STATUSES.Contains(Result.LastResponse.Value.StatusCode);
        }
    }
}
