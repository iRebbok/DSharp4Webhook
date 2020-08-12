using DSharp4Webhook.Core;
using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Linq;

namespace DSharp4Webhook.Internal
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    public readonly struct MessageMention : IMessageMention
    {
        [JsonProperty(PropertyName = "parse",
            ItemConverterType = typeof(StringEnumConverter),
            ItemConverterParameters = new object[] { typeof(LowerCaseNamingStrategy), new object[0], false })]
        public AllowedMention AllowedMention { get; }

        [JsonProperty("users")]
        public string[]? Users { get; }

        [JsonProperty("roles")]
        public string[]? Roles { get; }

        public MessageMention(AllowedMention mention) : this()
        {
            AllowedMention = mention;
        }

        public MessageMention(MessageMentionBuilder builder)
        {
            Contract.AssertNotNull(builder, nameof(builder));

            AllowedMention = builder.AllowedMention;
            Users = builder._users?.ToArray();
            Roles = builder._roles?.ToArray();
        }
    }
}
