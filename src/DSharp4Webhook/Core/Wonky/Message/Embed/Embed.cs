using DSharp4Webhook.Core;
using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Util;
using DSharp4Webhook.Util.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DSharp4Webhook.Internal.Embed
{
    /// <remarks>
    ///     Note that discord allows to use optional and nullable variables,
    ///     we use nullable and provide it as optional because
    ///     if we send it as null, it will return bad request (400).
    /// </remarks>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal readonly struct Embed : IEmbed
    {
        private readonly string? _title;
        private readonly EmbedType? _type;
        private readonly string? _description;
        private readonly string? _url;
        private readonly DateTimeOffset? _timestamp;
        private readonly uint? _color;
        private readonly IEmbedFooter? _footer;
        private readonly IEmbedImage? _image;
        private readonly IEmbedThumbnail? _thumbnail;
        private readonly IEmbedVideo? _video;
        private readonly IEmbedProvider? _provider;
        private readonly IEmbedAuthor? _author;
        private readonly ReadOnlyCollection<IEmbedField>? _fields;

        /// <exception cref="ArgumentOutOfRangeException">
        ///     Embed exceeds its limit.
        /// </exception>
        public Embed(EmbedBuilder builder)
        {
            Contract.AssertNotNull(builder, nameof(builder));

            int totalLength = builder.GetStringBuilder().Length +
                builder.GetFields().Sum(field => field.Value.Length + field.Name.Length) +
                (builder.Footer?.Text.Length ?? 0) + (builder.Author?.Name?.Length ?? 0);
            if (totalLength > WebhookProvider.MAX_EMBED_DATA_LENGTH)
                throw new ArgumentOutOfRangeException($"Embed exceed the embed limit of {WebhookProvider.MAX_EMBED_DATA_LENGTH} in length, see https://discord.com/developers/docs/resources/channel#embed-limits-limits");

            _title = builder.Title;
            _type = builder.Type;
            _description = builder.GetStringBuilder().ToString();
            _url = builder.Url;
            _timestamp = builder.Timestamp;
            _color = builder.Color;
            _footer = builder.Footer;
            _image = builder.Image;
            _thumbnail = builder.Thumbnail;
            _video = builder.Video;
            _provider = builder.Provider;
            _author = builder.Author;
            _fields = builder._fields?.ToArray().ToReadOnlyCollection();
        }

        #region Properties

        [JsonProperty(PropertyName = "title")]
        public string? Title
        {
            get => _title;
        }

        /// <remarks>
        ///     I don't think it will be perceived as usual
        ///     because the type name starts with a capital letter.
        /// </remarks>
        [JsonProperty(PropertyName = "type")]
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE1006 // Naming Styles
        private string? _Type => Type?.ToString().ToLowerInvariant();
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0051 // Remove unused private members

        public EmbedType? Type
        {
            get => _type;
        }

        [JsonProperty(PropertyName = "description")]
        public string? Description
        {
            get => _description;
        }

        [JsonProperty(PropertyName = "url")]
        public string? Url
        {
            get => _url;
        }

        /// <remarks>
        ///     Default serialization is ISO8601.
        /// </remarks>
        [JsonProperty(PropertyName = "timestamp")]
        public DateTimeOffset? Timestamp
        {
            get => _timestamp;
        }

        [JsonProperty(PropertyName = "color")]
        public uint? Color
        {
            get => _color;
        }

        [JsonProperty(PropertyName = "foter")]
        public IEmbedFooter? Footer
        {
            get => _footer;
        }

        [JsonProperty(PropertyName = "iamge")]
        public IEmbedImage? Image
        {
            get => _image;
        }

        [JsonProperty(PropertyName = "thumbnail")]
        public IEmbedThumbnail? Thumbnail
        {
            get => _thumbnail;
        }

        [JsonProperty(PropertyName = "video")]
        public IEmbedVideo? Video
        {
            get => _video;
        }

        [JsonProperty(PropertyName = "provider")]
        public IEmbedProvider? Provider
        {
            get => _provider;
        }

        [JsonProperty(PropertyName = "author")]
        public IEmbedAuthor? Author
        {
            get => _author;
        }

        [JsonProperty(PropertyName = "fields")]
        public ReadOnlyCollection<IEmbedField>? Fileds
        {
            get => _fields;
        }

        #endregion
    }
}
