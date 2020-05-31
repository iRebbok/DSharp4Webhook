using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSharp4Webhook.Core.Constructor
{
    /// <remarks>
    ///     Don't forget about <see cref="WebhookProvider.MAX_EMBED_DATA_LENGTH"/>.
    /// </remarks>
    public sealed class EmbedBuilder : IBuilder
    {
        private readonly StringBuilder _builder;

#nullable enable
        private string? _title;
        private EmbedType? _type;
        private string? _url;
        private DateTimeOffset? _timestamp;
        private uint? _color;
        private IEmbedFooter? _footer;
        private IEmbedImage? _image;
        private IEmbedThumbnail? _thumbnail;
        private IEmbedVideo? _video;
        private IEmbedProvider? _provider;
        private IEmbedAuthor? _author;
        private List<IEmbedField>? _fields;

        /// <summary>
        ///     Gets a new builder.
        /// </summary>
        public static EmbedBuilder New() => new EmbedBuilder();

        private EmbedBuilder()
        {
            _builder = new StringBuilder();
        }

        #region Properties

        /// <exception cref="ArgumentOutOfRangeException">
        ///     Exceeds the allowed length.
        /// </exception>
        public string? Title
        {
            get => _title;
            set
            {
                if (!(value is null))
                {
                    value = value.Trim();
                    Checks.CheckBounds(nameof(Title), $"Must be no more then {WebhookProvider.MAX_EMBED_TITLE_LENGTH} in length",
                        WebhookProvider.MAX_EMBED_TITLE_LENGTH, value.Length);
                    _title = value;
                }
                else
                    _title = value;
            }
        }

        public EmbedType? Type
        {
            get => _type;
            set => _type = value;
        }

        public string? Url
        {
            get => _url;
            set => _url = value;
        }

        public DateTimeOffset? Timestamp
        {
            get => _timestamp;
            set => _timestamp = value;
        }

        /// <remarks>
        ///     Use in conjunction with <see cref="ColorUtil.FromHex(string)"/>.
        /// </remarks>
        public uint? Color
        {
            get => _color;
            set => _color = value;
        }

        public IEmbedFooter? Footer
        {
            get => _footer;
            set => _footer = value;
        }

        public IEmbedImage? Image
        {
            get => _image;
            set => _image = value;
        }

        public IEmbedThumbnail? Thumbnail
        {
            get => _thumbnail;
            set => _thumbnail = value;
        }

        public IEmbedVideo? Video
        {
            get => _video;
            set => _video = value;
        }

        public IEmbedProvider? Provider
        {
            get => _provider;
            set => _provider = value;
        }

        public IEmbedAuthor? Author
        {
            get => _author;
            set => _author = value;
        }

        #endregion

#nullable restore

        #region Methods

        /// <summary>
        ///     Return the description string builder.
        /// </summary>
        public StringBuilder GetStringBuilder()
        {
            return _builder;
        }

        /// <summary>
        ///     Gets a list of fields.
        /// </summary>
        public List<IEmbedField> GetFields()
        {
            return _fields ??= new List<IEmbedField>();
        }

        /// <summary>
        ///     Adds text to the current description.
        /// </summary>
#nullable enable
        public EmbedBuilder Append(string? text)
#nullable restore
        {
            // If we put null, it will still be null in the description
            Checks.CheckBounds(nameof(text), $"Must be no more than {WebhookProvider.MAX_EMBED_DESCRIPTION_LENGTH} in length",
                WebhookProvider.MAX_EMBED_DESCRIPTION_LENGTH, text?.Length ?? 4, _builder.Length);
            _builder.Append(text ?? "null");

            return this;
        }

        /// <summary>
        ///     Tries to add text, 
        ///     without causing an exception when the bounds are exceeded.
        /// </summary>
#nullable enable
        public EmbedBuilder TryAppend(string? text)
#nullable restore
        {
            if (!Checks.CheckBoundsSafe(WebhookProvider.MAX_EMBED_DESCRIPTION_LENGTH, text?.Length ?? 4, _builder.Length))
                _builder.Append(text ?? "null");

            return this;
        }

        /// <summary>
        ///     Adds a new line to the current description.
        /// </summary>
        public EmbedBuilder AppendLine()
        {
            Checks.CheckBounds(null, $"Must be no more than {WebhookProvider.MAX_EMBED_DESCRIPTION_LENGTH} in length",
                WebhookProvider.MAX_EMBED_DESCRIPTION_LENGTH + 1, 1, _builder.Length);
            _builder.AppendLine();

            return this;
        }

        /// <summary>
        ///     Tries to add a new line to the current description,
        ///     without causing an exception when the bounds are exceeded.
        /// </summary>
        public EmbedBuilder TryAppendLine()
        {
            if (!Checks.CheckBoundsSafe(WebhookProvider.MAX_EMBED_DESCRIPTION_LENGTH + 1, 1, _builder.Length))
                _builder.AppendLine();

            return this;
        }

        /// <summary>
        ///     Adds text to the current description in a new line.
        /// </summary>
#nullable enable
        public EmbedBuilder AppendLine(string? text)
#nullable restore
        {
            // If we put null, it will still be null in the description, a line break is also added
            Checks.CheckBounds(nameof(text), $"Must be no more than {WebhookProvider.MAX_EMBED_DESCRIPTION_LENGTH} in length",
                WebhookProvider.MAX_EMBED_DESCRIPTION_LENGTH + 1, (text?.Length ?? 4) + 1, _builder.Length);
            _builder.AppendLine(text ?? "null");

            return this;
        }

        /// <summary>
        ///     Tries to add text to the current description in a new line,
        ///     without causing an exception when the bounds are exceeded.
        /// </summary>
#nullable enable
        public EmbedBuilder TryAppendLine(string? text)
#nullable restore
        {
            if (!Checks.CheckBoundsSafe(WebhookProvider.MAX_EMBED_DESCRIPTION_LENGTH + 1, text?.Length ?? 4 + 1, _builder.Length))
                _builder.AppendLine(text ?? "null");

            return this;
        }

        /// <summary>
        ///     Adds a field.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Field is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Field exceeds the allowed limit.
        /// </exception>
        public EmbedBuilder AddField(IEmbedField field)
        {
            Checks.CheckForNull(field, nameof(field));
            // Just safely get it instead
            if (GetFields().Count + 1 > WebhookProvider.MAX_EMBED_FIELDS_COUNT)
                throw new ArgumentOutOfRangeException();
            _fields.Add(field);

            return this;
        }

        /// <summary>
        ///     Same, but does not cause an exception.
        /// </summary>
        public EmbedBuilder TryAddField(IEmbedField field)
        {
            if (!(field is null) && GetFields().Count + 1 <= WebhookProvider.MAX_EMBED_FIELDS_COUNT)
                _fields.Add(field);

            return this;
        }

        /// <exception cref="ArgumentOutOfRangeException">
        ///     Embed exceeds its limit.
        /// </exception>
        public IEmbed Build()
        {
            return new Internal.Embed.Embed(this);
        }

        public void Reset()
        {
            _title = null;
            _type = null;
            _url = null;
            _timestamp = null;
            _color = null;
            _footer = null;
            _image = null;
            _thumbnail = null;
            _video = null;
            _provider = null;
            _author = null;
            _fields?.Clear();
            _builder.Clear();
        }

        #endregion
    }
}
