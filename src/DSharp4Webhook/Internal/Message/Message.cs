using DSharp4Webhook.Core;
using DSharp4Webhook.Serialization;
using DSharp4Webhook.Util;
using Newtonsoft.Json;
using System.Text;

namespace DSharp4Webhook.Internal
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class Message : IMessage
    {
        private readonly string _content;
        private readonly string _username;
        private readonly string _avatarUrl;
        private readonly bool _isTTS;
        private readonly IMessageMention _mention;

        private SerializeContext? _cache;

        [JsonProperty(PropertyName = "content")]
        public string Content { get => _content; }

        [JsonProperty(PropertyName = "tts")]
        public bool IsTTS { get => _isTTS; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get => _username; }

        [JsonProperty(PropertyName = "avatar_url")]
        public string AvatarUrl { get => _avatarUrl; }

        [JsonProperty(PropertyName = "allowed_mention")]
        public IMessageMention Mention { get => _mention; }

        public Message()
        {
            _mention = ConstructorProvider.GetDefaultMessageMention();
        }

        public Message(string message, IMessageMention messageMention, bool isTTS = false)
        {
            Checks.CheckForNull(messageMention, nameof(messageMention));
            _content = message;
            _isTTS = isTTS;
            _mention = messageMention;
        }

        public Message(IMessage source)
        {
            _username = source.Username;
            _avatarUrl = source.AvatarUrl;
            _content = source.Content;
            _isTTS = source.IsTTS;

            _mention = source.Mention;
        }

        public Message(MessageBuilder builder)
        {
            _content = builder.Builder.ToString();
            _username = builder.Username;
            _avatarUrl = builder.AvatarUrl;
            _isTTS = builder.IsTTS;
            _mention = builder.MessageMention;
        }

        public SerializeContext Serialize()
        {
            if (_cache.HasValue)
                return _cache.Value;

            return (_cache = new SerializeContext(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)))).Value;
        }
    }
}
