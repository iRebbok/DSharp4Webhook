using DSharp4Webhook.Core;
using DSharp4Webhook.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace DSharp4Webhook.Internal
{
    /// <remarks>
    ///     To serialize data for webhook modification.
    /// </remarks>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Include, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class ModifyContent : IWSerializable
    {
        [JsonProperty]
        public string name;

        /// <summary>
        ///     Image that is used for serialization.
        /// </summary>
        public IWebhookImage image;

        public ModifyContent()
        {
            // Immediately use an empty image
            image = WebhookImage.Empty;
        }

        public SerializeContext Serialize()
        {
            var jobject = JObject.FromObject(this);
            if (image != WebhookImage.Empty)
                jobject.Add("avatar", JToken.FromObject(image.ToUriScheme()));

            return new SerializeContext(Encoding.UTF8.GetBytes(jobject.ToString()));
        }
    }
}
