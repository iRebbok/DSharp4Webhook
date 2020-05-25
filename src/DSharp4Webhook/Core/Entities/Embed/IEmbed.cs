using System;

namespace DSharp4Webhook.Core.Embed
{
    /// <summary>
    ///     Embed object.
    /// </summary>
    public interface IEmbed
    {
        /// <summary>
        ///     Title of embed.
        /// </summary>
#nullable enable
        public string? Title { get; }
#nullable restore

        /// <summary>
        ///     Type of embed.
        /// </summary>
        /// <remarks>
        ///     Discord allows to use null.
        /// </remarks>
        public EmbedType? Type { get; }

        /// <summary>
        ///     Description of embed.
        /// </summary>
#nullable enable
        public string? Description { get; }
#nullable restore

        /// <summary>
        ///     Url of embed.
        /// </summary>
#nullable enable
        public string? Url { get; }
#nullable restore

        /// <summary>
        ///     Timestamp of embed content.
        /// </summary>
        public DateTimeOffset? Timestamp { get; }

        /// <summary>
        ///     Color code of the embed.
        /// </summary>
        public uint? Color { get; }

        /// <summary>
        ///     Footer information.
        /// </summary>
#nullable enable
        public IEmbedFooter? Footer { get; }
#nullable restore

        /// <summary>
        ///     Image information.
        /// </summary>
#nullable enable
        public IEmbedImage? Image { get; }
#nullable restore

        /// <summary>
        ///     Thumbnail information.
        /// </summary>
#nullable enable
        public IEmbedThumbnail? Thumbnail { get; }
#nullable restore

        /// <summary>
        ///     Video information.
        /// </summary>
#nullable enable
        public IEmbedVideo? Video { get; }
#nullable restore

        /// <summary>
        ///     Provider information.
        /// </summary>
#nullable enable
        public IEmbedProvider? Provider { get; }
#nullable restore

        /// <summary>
        ///     Author information.
        /// </summary>
#nullable enable
        public IEmbedAuthor? Author { get; }
#nullable restore

        /// <summary>
        ///     Fields information.
        /// </summary>
#nullable enable
        public IEmbedField[]? Fileds { get; }
#nullable restore
    }
}
