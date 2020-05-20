using System.Collections.Generic;

namespace DSharp4Webhook.Core
{
    /// <remarks>
    ///     Provided as 'allowed_mentions' in the message object.
    /// </remarks>
    public interface IMessageMention
    {
        /// <summary>
        ///     Allowed types of mentions.
        /// </summary>
        public AllowedMention AllowedMention { get; set; }

        /// <summary>
        ///     Users allowed to be mentioned.
        /// </summary>
        public List<string> Users { get; }

        /// <summary>
        ///     Roles allowed to be mentioned.
        /// </summary>
        public List<string> Roles { get; }
    }
}
