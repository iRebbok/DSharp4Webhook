using DSharp4Webhook.Core;
using DSharp4Webhook.Serialization;
using DSharp4Webhook.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace DSharp4Webhook.Internal
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class Message : IMessage
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

        private string username;
        [JsonProperty(PropertyName = "username")]
        public string Username
        {
            get => username?.Length <= WebhookProvider.MAX_NICKNAME_LENGTH || username?.Length >= WebhookProvider.MIN_NICKNAME_LENGTH ? username : null;
            set
            {
                if (value != null)
                {
                    if ((value = value.Trim()).Length >= WebhookProvider.MIN_NICKNAME_LENGTH || value.Length <= WebhookProvider.MAX_NICKNAME_LENGTH)
                        username = value;
                    else
                        throw new ArgumentOutOfRangeException(nameof(Username), $"Must be between {WebhookProvider.MIN_NICKNAME_LENGTH} and {WebhookProvider.MAX_NICKNAME_LENGTH} in length.");
                }
                // Null set possible
                else
                    username = value;
            }
        }

        [JsonProperty(PropertyName = "avatar_url")]
        public string AvatarUrl { get; set; } = null;

        private IMessageMention _mention;
        public IMessageMention Mention { get => _mention; set => _mention = value ?? _mention; }

        public Message() { }

        public Message(string message, IMessageMention messageMention, bool isTTS = false)
        {
            Checks.CheckForNull(messageMention, nameof(messageMention));
            Content = message;
            IsTTS = isTTS;
            _mention = messageMention;
        }

        public Message(IMessage source)
        {
            Username = source.Username;
            AvatarUrl = source.AvatarUrl;
            Content = source.Content;
            IsTTS = source.IsTTS;

            _mention = source.Mention;
        }

        public SerializeContext Serialize()
        {
            var jobject = JObject.FromObject(this);
            jobject.Add("allowed_mentions", JToken.Parse(Encoding.ASCII.GetString(_mention.Serialize().Content)));
            return new SerializeContext(Encoding.UTF8.GetBytes(jobject.ToString()));
        }
    }
}
