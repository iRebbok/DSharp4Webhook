using DSharp4Webhook.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DSharp4Webhook.Internal
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemRequired = Required.Default)]
    internal sealed class WebhookInfo : IWebhookInfo
    {
        #region Fields

        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(StringEnumConverter))]
        private WebhookType type;
        [JsonProperty(PropertyName = "id")]
        private string id;
        [JsonProperty(PropertyName = "name")]
        private string name;
        [JsonProperty(PropertyName = "avatar")]
        private string avatarId;
        [JsonProperty(PropertyName = "channel_id")]
        private string channelId;
        [JsonProperty(PropertyName = "guild_id")]
        private string guildId;
        [JsonProperty(PropertyName = "token")]
        private string token;

        #endregion

        #region Properties

        public WebhookType Type { get => type; }
        public string Id { get => id; }
        public ulong IdULong { get => ulong.Parse(id); }
        public string Name { get => name; }
        public string AvatarId { get => avatarId; }
        public string AvatarUrl { get => avatarId == null ? null : string.Format(WebhookProvider.WebhookBaseAvatarUrl, id, avatarId, avatarId.StartsWith("a_") ? "gif" : "png"); }
        public string ChannelId { get => channelId; }
        public ulong ChannelIdUlong { get => ulong.Parse(channelId); }
        public string GuildId { get => guildId; }
        public ulong GuildIdULong { get => ulong.Parse(guildId); }
        public string Token { get => token; }

        #endregion
    }
}
