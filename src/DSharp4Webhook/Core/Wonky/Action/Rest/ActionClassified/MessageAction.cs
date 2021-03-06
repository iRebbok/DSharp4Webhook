using DSharp4Webhook.Actions;
using DSharp4Webhook.Actions.Rest;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest;
using DSharp4Webhook.Rest.Manipulation;
using System.Linq;
using System.Threading.Tasks;

namespace DSharp4Webhook.Internal
{
    internal sealed class MessageAction : BaseRestAction<IRestResult>, IMessageAction
    {
        public IMessage Message { get; }

        public MessageAction(IMessage message, IWebhook webhook, RestSettings restSettings) : base(webhook, restSettings)
        {
            Message = message;
        }

        public override async Task<bool> ExecuteAsync()
        {
            CheckExecution();
            Result = new RestResult(await Webhook.RestProvider.POST(Webhook.GetWebhookUrl(), Message.Serialize(), RestSettings));
            SettingRateLimit();
            return Result.LastResponse.HasValue && BaseRestProvider.POST_ALLOWED_STATUSES.Contains(Result.LastResponse.Value.StatusCode);
        }
    }
}
