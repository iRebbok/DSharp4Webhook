using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Util;
using Newtonsoft.Json;

namespace DSharp4Webhook.Internal.Embed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class EmbedProvider : IEmbedProvider
    {
#nullable enable
        private readonly string? _name;
        private readonly string? _url;

        public EmbedProvider(EmbedProviderBuilder builder)
        {
            Checks.CheckForNull(builder, nameof(builder));

            _name = builder.Name;
            _url = builder.Url;
        }

        [JsonProperty(PropertyName = "name")]
        public string? Name
        {
            get => _name;
        }

        [JsonProperty(PropertyName = "url")]
        public string? Url
        {
            get => _url;
        }
#nullable restore
    }
}
