using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Util;
using Newtonsoft.Json;

namespace DSharp4Webhook.Internal.Embed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class EmbedVideo : IEmbedVideo
    {
#nullable enable
        private readonly uint? _height;
        private readonly uint? _width;
        private readonly string? _url;

        public EmbedVideo(EmbedVideoBuilder builder)
        {
            Checks.CheckForNull(builder, nameof(builder));

            _height = builder.Height;
            _width = builder.Width;
            _url = builder.Url;
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
#nullable restore
    }
}