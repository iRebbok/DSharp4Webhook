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
        public AllowedMention AllowedMention { get; }

        /// <summary>
        ///     Users allowed to be mentioned.
        /// </summary>
        public string[] Users { get; }

        /// <summary>
        ///     Roles allowed to be mentioned.
        /// </summary>
        public string[] Roles { get; }
    }
}
