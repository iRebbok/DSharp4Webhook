using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Util;
using Newtonsoft.Json;

namespace DSharp4Webhook.Internal.Embed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class EmbedField : IEmbedField
    {
        private readonly string _name;
        private readonly string _value;
        private readonly bool? _inline;

        public EmbedField(EmbedFieldBuilder builder)
        {
            Checks.CheckForNull(builder, nameof(builder));

            _name = builder.Name!;
            _value = builder.Value!;
            _inline = builder.Inline;
        }

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get => _name;
        }

        [JsonProperty(PropertyName = "value")]
        public string Value
        {
            get => _value;
        }

        [JsonProperty(PropertyName = "inline")]
        public bool? Inline
        {
            get => _inline;
        }
    }
}
