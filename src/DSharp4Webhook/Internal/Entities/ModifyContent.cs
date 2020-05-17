using DSharp4Webhook.Serialization;
using Newtonsoft.Json;
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

        // Immediately use an empty image
        [JsonProperty]
        public string avatar = WebhookImage.Empty.ToUriScheme();

        public SerializeContext Serialize()
        {
            return new SerializeContext(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this)));
        }
    }
}
