using DSharp4Webhook.Entities;
using System;

namespace DSharp4Webhook.Internal
{
    internal class WebhookMessageImpl : WebhookDataImpl, IWebhookMessage
    {
        private string content;
        public string Content
        {
            get => content?.Length <= 2000 && content?.Length > 0 ? content : null;
            set
            {
                if (value != null)
                {
                    if ((value = value.Trim()).Length <= 2000)
                        content = value;
                    else
                        throw new ArgumentOutOfRangeException(nameof(Content), "It must not be more than 2000 in length.");
                }
                // Null set possible
                else
                    content = value;
            }
        }

        public ulong DeliveryId { get; internal set; } = 0L;

        public bool IsTTS { get; set; } = false;

        public WebhookMessageImpl(string message, bool isTTS = false)
        {
            Content = message;
            IsTTS = isTTS;
        }

        public WebhookMessageImpl(IWebhookMessage source)
        {
            Username = source.Username;
            AvatarUrl = source.AvatarUrl;
            Content = source.Content;
            IsTTS = source.IsTTS;
        }

    }
}
