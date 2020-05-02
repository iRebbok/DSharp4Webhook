using DSharp4Webhook.Entities;
using System;

namespace DSharp4Webhook.Internal
{
    internal class WebhookDataImpl : IBaseWebhookData
    {
        private string username;
        public string Username
        {
            get => username?.Length <= 80 && username?.Length > 0 ? username : null;
            set
            {
                if (value != null)
                {
                    if ((value = value.Trim()).Length > 0 && value.Length <= 80)
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
