using DSharp4Webhook.Core.Embed;
using Newtonsoft.Json;

namespace DSharp4Webhook.Internal.Embed
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class EmbedField : IEmbedField
    {
        private readonly string _name;
        private readonly string _value;
        private readonly bool? _inline;

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
