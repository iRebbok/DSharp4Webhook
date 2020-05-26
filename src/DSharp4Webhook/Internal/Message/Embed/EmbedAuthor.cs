using DSharp4Webhook.Core.Embed;
using Newtonsoft.Json;

namespace DSharp4Webhook.Internal.Embed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class EmbedAuthor : IEmbedAuthor
    {
#nullable enable
        private readonly string? _name;
        private readonly string? _iconUrl;
        private readonly string? _proxyIconUrl;
        private readonly string? _url;

        [JsonProperty(PropertyName = "name")]
        public string? Name
        {
            get => _name;
        }

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

        [JsonProperty(PropertyName = "url")]
        public string? Url
        {
            get => _url;
        }
#nullable restore
    }
}
