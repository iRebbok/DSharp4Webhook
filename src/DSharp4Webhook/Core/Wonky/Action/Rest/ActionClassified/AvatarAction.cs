using DSharp4Webhook.Actions.Rest;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest;
using System.Threading.Tasks;

namespace DSharp4Webhook.Internal
{
    internal sealed class AvatarAction : BaseRestAction<IAvatarResult>, IAvatarAction
    {
        private IWebhookInfo? _webhookInfo;

        public AvatarAction(IWebhook webhook, RestSettings restSettings, IWebhookInfo? webhookInfo = null) : base(webhook, restSettings)
        {
            _webhookInfo = webhookInfo;
        }

        public override async Task<bool> ExecuteAsync()
        {
            CheckExecution();

            string? avatarUrl;
            if (_webhookInfo is null)
            {
                var infoAction = Webhook.GetInfo();
                await infoAction.ExecuteAsync();
                _webhookInfo = infoAction.Result.WebhookInfo;
            }

            avatarUrl = _webhookInfo.AvatarUrl;

            if (string.IsNullOrEmpty(avatarUrl))
                return false;

            if (!string.IsNullOrEmpty(avatarUrl))
            {
                var responses = await Webhook.RestProvider.GET(avatarUrl!, RestSettings);
                var lastResponse = responses[responses.Length - 1];
                Result = new AvatarResult(new WebhookImage(lastResponse.Data!), responses);
                return true;
            }
            return false;
        }
    }
}
