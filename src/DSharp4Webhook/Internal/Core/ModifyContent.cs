using DSharp4Webhook.Core;
using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Serialization;
using DSharp4Webhook.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace DSharp4Webhook.Internal
{
    /// <remarks>
    ///     To serialize data for webhook modification.
    /// </remarks>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Include, MemberSerialization = MemberSerialization.OptIn)]
    internal struct ModifyContent : IModifyContent
    {
        public string Name => name;
        public IWebhookImage? Image => image;

        [JsonProperty]
        public string name;

        /// <summary>
        ///     Image that is used for serialization.
        /// </summary>
        public IWebhookImage? image;

        public ModifyContent(ModifyContentBuilder builder)
        {
            Checks.CheckForNull(builder, nameof(builder));

            name = builder.Name!;
            image = builder.Image;
        }

        public SerializeContext Serialize()
        {
            var jobject = JObject.FromObject(this);
            if (!(image is null) && !WebhookImage.Empty.Equals(image))
                jobject.Add("avatar", JToken.FromObject(image.ToUriScheme()));

            return new SerializeContext(Encoding.UTF8.GetBytes(jobject.ToString()));
        }
    }
}
