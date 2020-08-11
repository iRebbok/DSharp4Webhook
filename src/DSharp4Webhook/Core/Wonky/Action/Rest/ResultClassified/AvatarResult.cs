using DSharp4Webhook.Actions.Rest;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest;

namespace DSharp4Webhook.Internal
{
    internal sealed class AvatarResult : RestResult, IAvatarResult
    {
        public IWebhookImage Image { get; }

        public AvatarResult(IWebhookImage image, RestResponse[] responses) : base(responses)
        {
            Image = image;
        }
    }
}
