using DSharp4Webhook.Core.Serialization;
using Newtonsoft.Json;
using System;
using System.Text;

namespace DSharp4Webhook.Core
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    public class WebhookMessage : WebhookMessageInfo, IWebhookMessage
    {
        private string content;

        [JsonProperty(PropertyName = "content")]
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

        [JsonProperty(PropertyName = "tts")]
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

        public SerializeContext Serialize()
        {
            return new SerializeContext(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)));
        }
    }
}
