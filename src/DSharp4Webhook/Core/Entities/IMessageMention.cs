using DSharp4Webhook.Serialization;

namespace DSharp4Webhook.Core
{
    /// <remarks>
    ///     Provided as 'allowed_mentions' in the message object.
    /// </remarks>
    public interface IMessageMention : IWSerializable
    {
        /// <summary>
        ///     Allowed types of mentions.
        /// </summary>
        public AllowedMention AllowedMention { get; set; }

        /// <summary>
        ///     Users allowed to be mentioned.
        /// </summary>
        public string[] Users { get; set; }

        /// <summary>
        ///     Roles allowed to be mentioned
        /// </summary>
        public string[] Roles { get; set; }
    }
}
