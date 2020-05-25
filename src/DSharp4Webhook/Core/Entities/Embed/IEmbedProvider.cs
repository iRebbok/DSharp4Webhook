using DSharp4Webhook.Core.Embed.Subtypes;

namespace DSharp4Webhook.Core.Embed
{
    /// <summary>
    ///     Provider of embed.
    /// </summary>
    public interface IEmbedProvider : IUrlable
    {
        /// <summary>
        ///     Name of provider.
        /// </summary>
#nullable enable
        public string? Name { get; }
#nullable restore
    }
}
