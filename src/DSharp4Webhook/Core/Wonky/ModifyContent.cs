using DSharp4Webhook.Core;
using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Serialization;
using DSharp4Webhook.Util;
using Newtonsoft.Json.Linq;
using System.Text;

namespace DSharp4Webhook.Internal
{
    /// <remarks>
    ///     To serialize data for webhook modification.
    /// </remarks>
    internal struct ModifyContent : IModifyContent
    {
        public string? Name => name;
        public IWebhookImage? Image => image;

        private readonly string? name;

        /// <summary>
        ///     Image that is used for serialization.
        /// </summary>
        private readonly IWebhookImage? image;

        /// <summary>
        ///     Since we can also only change the image,
        ///     we simply ignore the name if it's empty so as not to cause an error
        /// </summary>
        private readonly bool _ignoreName;

        public ModifyContent(string name, IWebhookImage? image, bool ignoreName)
        {
            this.image = image;
            this.name = name;
            _ignoreName = ignoreName;
        }

        public ModifyContent(ModifyContentBuilder builder)
        {
            Contract.AssertNotNull(builder, nameof(builder));

            name = builder.Name;
            image = builder.Image;
            _ignoreName = false;
        }

        public SerializationContext Serialize()
        {
            // Complete controlled serialization
            var jobject = new JObject();
            if (!WebhookImage.Empty.Equals(image))
                jobject.Add("avatar", image is null ? null : JToken.FromObject(image.ToUriScheme()));

            if (!_ignoreName)
                jobject.Add("name", JToken.FromObject(name!));

            return new SerializationContext(Encoding.UTF8.GetBytes(jobject.ToString()));
        }
    }
}
