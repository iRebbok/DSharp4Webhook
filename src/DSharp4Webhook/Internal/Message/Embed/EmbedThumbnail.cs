using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Util;
using Newtonsoft.Json;

namespace DSharp4Webhook.Internal.Embed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class EmbedThumbnail : IEmbedThumbnail
    {
        private readonly uint? _height;
        private readonly uint? _width;
        private readonly string? _url;
        private readonly string? _proxyUrl;

        public EmbedThumbnail(EmbedThumbnailBuilder builder)
        {
            Checks.CheckForNull(builder, nameof(builder));

            _height = builder.Height;
            _width = builder.Width;
            _url = builder.Url;
            _proxyUrl = builder.ProxyUrl;
        }

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
    }
}
