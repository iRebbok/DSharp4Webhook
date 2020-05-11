using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Core;
using System.Text;
using System.Threading.Tasks;

namespace DSharp4Webhook.Internal
{
    internal sealed class AvatarAction : BaseRestAction<IAvatarResult>, IAvatarAction
    {
        private IWebhookInfo _webhookInfo;

        public AvatarAction(IWebhook webhook, IWebhookInfo webhookInfo = null) : base(webhook)
        {
            _webhookInfo = webhookInfo;
        }

        public override async Task<bool> ExecuteAsync()
        {
            string avatarUrl;
            if (_webhookInfo == null)
            {
                var infoAction = Webhook.Info();
                await infoAction.ExecuteAsync();
                avatarUrl = infoAction.Result.WebhookInfo.AvatarUrl;
            }
            else
                avatarUrl = _webhookInfo.AvatarUrl;

            if (!string.IsNullOrEmpty(avatarUrl))
            {
                var responses = await Webhook.RestProvider.GET(avatarUrl, 1);
                var lastResponse = responses[responses.Length - 1];
                Result = new AvatarResult(new WebhookImage(Encoding.ASCII.GetBytes(lastResponse.Content)), responses);
                return true;
            }
            return false;
        }
    }
}
