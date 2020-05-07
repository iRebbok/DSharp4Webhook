
using System;

namespace DSharp4Webhook.Core
{
    public sealed class WebhookMessageInfo : IWebhookMessageInfo
    {
        private string username;
        public string Username
        {
            get => username?.Length <= WebhookProvider.MAX_NICKNAME_LENGTH && username?.Length >= WebhookProvider.MIX_NICKNAME_LENGHT ? username : null;
            set
            {
                if (value != null)
                {
                    if ((value = value.Trim()).Length >= WebhookProvider.MIX_NICKNAME_LENGHT && value.Length <= WebhookProvider.MAX_NICKNAME_LENGTH)
                        username = value;
                    else
                        throw new ArgumentOutOfRangeException(nameof(Username), "Must be between 1 and 80 in length.");
                }
                // Null set possible
                else
                    username = value;
            }
        }

        public string AvatarUrl { get; set; } = null;
    }
}
