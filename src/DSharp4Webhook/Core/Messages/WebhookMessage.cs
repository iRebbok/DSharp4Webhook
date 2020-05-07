using System;

namespace DSharp4Webhook.Core
{
    public class WebhookMessage : WebhookMessageInfo, IWebhookMessage
    {
        private string content;
        public string Content
        {
            get => content?.Length <= WebhookProvider.MAX_CONTENT_LENGTH && content?.Length > 0 ? content : null;
            set
            {
                if (value != null)
                {
                    if ((value = value.Trim()).Length <= WebhookProvider.MAX_CONTENT_LENGTH)
                        content = value;
                    else
                        throw new ArgumentOutOfRangeException(nameof(Content), "It must not be more than 2000 in length.");
                }
                // Null set possible
                else
                    content = value;
            }
        }

        public bool IsTTS { get; set; } = false;

        public WebhookMessage() { }

        public WebhookMessage(string message, bool isTTS = false)
        {
            Content = message;
            IsTTS = isTTS;
        }

        public WebhookMessage(IWebhookMessage source)
        {
            Username = source.Username;
            AvatarUrl = source.AvatarUrl;
            Content = source.Content;
            IsTTS = source.IsTTS;
        }

    }
}
