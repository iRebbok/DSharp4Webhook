using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Util;
using Newtonsoft.Json;

namespace DSharp4Webhook.Internal.Embed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.Fields)]
    internal readonly struct EmbedFooter : IEmbedFooter
    {
        private readonly string _text;
        private readonly string? _iconUrl;
        private readonly string? _proxyIconUrl;

        public EmbedFooter(EmbedFooterBuilder builder)
        {
            Contract.AssertNotNull(builder, nameof(builder));

            _text = builder.Text!;
            _iconUrl = builder.IconUrl;
            _proxyIconUrl = builder.ProxyIconUrl;
        }

        [JsonProperty(PropertyName = "text")]
        public string Text
        {
            get => _text;
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
    }
}
