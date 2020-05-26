using DSharp4Webhook.Core.Embed;
using Newtonsoft.Json;

namespace DSharp4Webhook.Internal.Embed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.Fields)]
    internal sealed class EmbedFooter : IEmbedFooter
    {
        private readonly string _text;
#nullable enable
        private readonly string? _iconUrl;
        private readonly string? _proxyIconUrl;
#nullable restore

        [JsonProperty(PropertyName = "text")]
        public string Text
        {
            get => _text;
        }

#nullable enable
        [JsonProperty(PropertyName = "icon_url")]
        public string? IconUrl
        {
            get => _iconUrl;
        }

        [JsonProperty(PropertyName = "proxy_icon_url")]
        public string? ProxyIconUrl
        {
            get => _proxyIconUrl;
        }
#nullable restore
    }
}
