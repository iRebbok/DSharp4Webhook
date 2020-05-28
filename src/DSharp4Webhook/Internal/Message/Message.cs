using DSharp4Webhook.Core;
using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Serialization;
using DSharp4Webhook.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private readonly IEmbed[] _embeds;
        private readonly IReadOnlyDictionary<string, byte[]> _files;

        private SerializeContext? _cache;

        #region Properties

        [JsonProperty(PropertyName = "content")]
        public string Content { get => _content; }

        [JsonProperty(PropertyName = "tts")]
        public bool IsTTS { get => _isTTS; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get => _username; }

        [JsonProperty(PropertyName = "avatar_url")]
        public string AvatarUrl { get => _avatarUrl; }

        [JsonProperty(PropertyName = "embeds")]
        public IEmbed[] Embeds { get => _embeds; }

        [JsonProperty(PropertyName = "allowed_mention")]
        public IMessageMention Mention { get => _mention; }

        public IReadOnlyDictionary<string, byte[]> Files { get => _files; }

        #endregion

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

        public Message(IEnumerable<IEmbed> embeds, IMessageMention messageMention)
        {
            Checks.CheckForNull(messageMention, nameof(messageMention));

            _embeds = embeds.ToArray();
            _mention = messageMention;
        }

        public Message(IMessage source)
        {
            Checks.CheckForAttachments(source.Files);

            _username = source.Username;
            _avatarUrl = source.AvatarUrl;
            _content = source.Content;
            _isTTS = source.IsTTS;

            _mention = source.Mention;
            _files = source.Files;
            _embeds = source.Embeds;
        }

        /// <exception cref="ArgumentOutOfRangeException">
        ///     Message exceeds its limit.
        /// </exception>
        public Message(MessageBuilder builder)
        {
            Checks.CheckForAttachments(builder.Files);

            if (builder.Embeds.Count > WebhookProvider.MAX_EMBED_COUNT ||
                builder.Builder.Length > WebhookProvider.MAX_CONTENT_LENGTH)
                throw new ArgumentOutOfRangeException();

            _content = builder.Builder.ToString();
            _username = builder.Username;
            _avatarUrl = builder.AvatarUrl;
            _isTTS = builder.IsTTS;
            _mention = builder.MessageMention;
            _files = builder._files == null ? null : new ReadOnlyDictionary<string, byte[]>(builder._files);
            _embeds = builder._embeds == null ? null : builder._embeds.ToArray();
        }

        public SerializeContext Serialize()
        {
            if (_cache.HasValue)
                return _cache.Value;

            return (_cache = new SerializeContext(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)), _files?.ToDictionary(key => key.Key, value => value.Value))).Value;
        }
    }
}
