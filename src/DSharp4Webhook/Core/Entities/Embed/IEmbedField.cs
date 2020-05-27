namespace DSharp4Webhook.Core.Embed
{
    /// <summary>
    ///     Field of embed.
    /// </summary>
    public interface IEmbedField
    {
        /// <summary>
        ///     Name of the field.
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Value of the field.
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     Whether or not this field should display inline.
        /// </summary>
        public bool? Inline { get; }
    }
}
