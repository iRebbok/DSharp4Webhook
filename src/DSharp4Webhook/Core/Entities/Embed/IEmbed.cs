using System;
using System.Collections.ObjectModel;

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
        public string? Title { get; }

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
        public string? Description { get; }

        /// <summary>
        ///     Url of embed.
        /// </summary>
        public string? Url { get; }

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
        public IEmbedFooter? Footer { get; }

        /// <summary>
        ///     Image information.
        /// </summary>
        public IEmbedImage? Image { get; }

        /// <summary>
        ///     Thumbnail information.
        /// </summary>
        public IEmbedThumbnail? Thumbnail { get; }

        /// <summary>
        ///     Video information.
        /// </summary>
        public IEmbedVideo? Video { get; }

        /// <summary>
        ///     Provider information.
        /// </summary>
        public IEmbedProvider? Provider { get; }

        /// <summary>
        ///     Author information.
        /// </summary>
        public IEmbedAuthor? Author { get; }

        /// <summary>
        ///     Fields information.
        /// </summary>
        public ReadOnlyCollection<IEmbedField>? Fileds { get; }
    }
}
