using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Util;
using Newtonsoft.Json;

namespace DSharp4Webhook.Internal.Embed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    public readonly struct EmbedAuthor : IEmbedAuthor
    {
        private readonly string? _name;
        private readonly string? _iconUrl;
        private readonly string? _proxyIconUrl;
        private readonly string? _url;

        public EmbedAuthor(EmbedAuthorBuilder builder)
        {
            Contract.AssertNotNull(builder, nameof(builder));

            _name = builder.Name;
            _iconUrl = builder.IconUrl;
            _proxyIconUrl = builder.ProxyIconUrl;
            _url = builder.Url;
        }

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
    }
}
