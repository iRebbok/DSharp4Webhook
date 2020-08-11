using DSharp4Webhook.Core.Embed.Subtypes;

namespace DSharp4Webhook.Core.Embed
{
    /// <summary>
    ///     Footer of embed.
    /// </summary>
    public interface IEmbedFooter : IIconable
    {
        /// <summary>
        ///     Footer text.
        /// </summary>
        public string Text { get; }
    }
}
