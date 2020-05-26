using DSharp4Webhook.Core;
using DSharp4Webhook.Core.Embed;
using Newtonsoft.Json;
using System;

namespace DSharp4Webhook.Internal.Embed
{
    /// <remarks>
    ///     Note that discord allows to use optional and nullable variables,
    ///     we use nullable and provide it as optional because
    ///     if we send it as null, it will return bad request (400).
    /// </remarks>
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class Embed : IEmbed
    {
#nullable enable
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
        private readonly IEmbedField[]? _fields;

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
        private string? _Type => Type?.ToString().ToLowerInvariant();

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
        public IEmbedField[]? Fileds
        {
            get => _fields;
        }

        #endregion
#nullable restore
    }
}
