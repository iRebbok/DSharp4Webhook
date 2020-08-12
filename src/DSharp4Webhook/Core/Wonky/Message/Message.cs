using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Serialization;
using DSharp4Webhook.Util;
using Newtonsoft.Json;
using System.Text;

namespace DSharp4Webhook.Core
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    public readonly struct Message : IMessage
    {
        #region Properties

        [JsonProperty(PropertyName = "content")]
        public string? Content { get; }

        [JsonProperty(PropertyName = "tts")]
        public bool IsTTS { get; }

        [JsonProperty(PropertyName = "username")]
        public string? Username { get; }

        [JsonProperty(PropertyName = "avatar_url")]
        public string? AvatarUrl { get; }

        [JsonProperty(PropertyName = "embeds")]
        public IEmbed[]? Embeds { get; }

        [JsonProperty(PropertyName = "allowed_mention")]
        public IMessageMention Mention { get; }

        public FileEntry[]? Attachments { get; }

        #endregion

        public Message(string message, IMessageMention messageMention, bool isTTS = false) : this()
        {
            Contract.AssertArgumentNotTrue(string.IsNullOrEmpty(message), nameof(message));
            Contract.AssertNotNull(messageMention, nameof(messageMention));

            Content = message;
            Mention = messageMention;
            IsTTS = isTTS;
        }

        public Message(IEmbed[] embeds, IMessageMention messageMention) : this()
        {
            Contract.AssertNotNull(embeds, nameof(embeds));
            Contract.AssertNotNull(messageMention, nameof(messageMention));

            Embeds = embeds;
            Mention = messageMention;
        }

        public Message(IMessage source)
        {
            Contract.AssertNotNull(source, nameof(source));

            Username = source.Username;
            AvatarUrl = source.AvatarUrl;
            Content = source.Content;
            IsTTS = source.IsTTS;

            Mention = source.Mention;
            Attachments = source.Attachments;
            Embeds = source.Embeds;
        }

        public Message(MessageBuilder builder)
        {
            Contract.AssertNotNull(builder, nameof(builder));

            Contract.AssertSafeBounds(
                builder.Username?.Length ?? WebhookProvider.MIN_NICKNAME_LENGTH,
                WebhookProvider.MIN_NICKNAME_LENGTH, WebhookProvider.MAX_NICKNAME_LENGTH,
                nameof(builder.Username));

            Contract.AssertNotNull(builder.MessageMention, nameof(builder.MessageMention));
            Contract.AssertSafeBounds(builder.Builder.Length, -1, WebhookProvider.MAX_CONTENT_LENGTH, "text");

            Contract.AssertArgumentNotTrue(
                !(builder.Builder.Length > 0 || !(builder._embeds is null) || !(builder._files is null)),
                "one of the attachments or embed or text is required");

            Contract.AssertSafeBounds(builder._embeds?.Count ?? 0, -1, WebhookProvider.MAX_EMBED_COUNT, "embeds");

            if (!(builder._files is null))
            {
                Contract.AssertSafeBounds(builder._files.Count, -1, WebhookProvider.MAX_ATTACHMENTS, "attachments");
                Contract.AssertNotOversize(builder._files);
            }

            Username = builder.Username;
            IsTTS = builder.IsTTS;
            AvatarUrl = builder.AvatarUrl;
            Mention = builder.MessageMention!;
            Content = builder.Builder.Length > 0 ? builder.Builder.ToString() : null;
            Embeds = builder._embeds?.ToArray();
            Attachments = builder._files?.ToArray();
        }

        public SerializationContext Serialize() => new SerializationContext(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)), Attachments);
    }
}
