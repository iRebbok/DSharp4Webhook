using DSharp4Webhook.Core.Embed;
using Newtonsoft.Json;

namespace DSharp4Webhook.Internal.Embed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class EmbedThumbnail : IEmbedThumbnail
    {
#nullable enable
        private readonly uint? _height;
        private readonly uint? _width;
        private readonly string? _url;
        private readonly string? _proxyUrl;

        [JsonProperty(PropertyName = "height")]
        public uint? Height
        {
            get => _height;
        }

        [JsonProperty(PropertyName = "width")]
        public uint? Width
        {
            get => _width;
        }

        [JsonProperty(PropertyName = "url")]
        public string? Url
        {
            get => _url;
        }

        [JsonProperty(PropertyName = "proxy_url")]
        public string? ProxyUrl
        {
            get => _proxyUrl;
        }
#nullable restore
    }
}
