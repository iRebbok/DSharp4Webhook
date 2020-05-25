using DSharp4Webhook.Core.Embed.Subtypes;

namespace DSharp4Webhook.Core.Embed
{
    /// <summary>
    ///     Author of embed.
    /// </summary>
    public interface IEmbedAuthor : IIconable, IUrlable
    {
        /// <summary>
        ///     Name of author.
        /// </summary>
#nullable enable
        public string? Name { get; }
#nullable restore
    }
}
