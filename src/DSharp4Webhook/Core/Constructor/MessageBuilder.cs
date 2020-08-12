using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.InternalExceptions;
using DSharp4Webhook.Serialization;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSharp4Webhook.Core.Constructor
{
    /// <summary>
    ///     Message builder that allows you to create messages for webhook.
    /// </summary>
    public sealed class MessageBuilder : IBuilder<Message>
    {
        private string? _username;
        private IMessageMention? _mention;

        // Don't create extra objects after the build
        internal List<IEmbed>? _embeds;
        internal List<FileEntry>? _files;

        /// <summary>
        ///     Default line terminator.
        /// </summary>
        public const string LINE_TERMINATOR = "\n";

        #region Properties

        /// <summary>
        ///     Gets the message builder for this builder.
        /// </summary>
        public StringBuilder Builder { get; } = new StringBuilder(WebhookProvider.MAX_CONTENT_LENGTH);

        /// <summary>
        ///     Gets a list of embeds.
        /// </summary>
        public List<IEmbed> Embeds { get => _embeds ??= new List<IEmbed>(10); }

        /// <summary>
        ///     Whether the TTS determines this message or not.
        /// </summary>
        public bool IsTTS { get; set; }

        /// <summary>
        ///     Username of the webhook that will be used for this message.
        /// </summary>
        public string? Username
        {
            get => _username;
            set
            {
                Contract.AssertNotNull(value, nameof(Username));

                value = value!.Trim();

                Contract.AssertSafeBounds(
                    value,
                    WebhookProvider.MIN_NICKNAME_LENGTH,
                    WebhookProvider.MAX_NICKNAME_LENGTH,
                    nameof(Username),
                    $"Must be between {WebhookProvider.MIN_NICKNAME_LENGTH} and {WebhookProvider.MAX_NICKNAME_LENGTH} in length");

                _username = value;
            }
        }

        /// <summary>
        ///     An image that will use webhook on this message.
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        ///     Allowed mentions in the message.
        /// </summary>
        public IMessageMention? MessageMention
        {
            // If the setted value is null, then initial value is used
            get => _mention;
            set
            {
                Contract.AssertNotNull(value, nameof(MessageMention));
                _mention = value;
            }
        }

        /// <summary>
        ///     Message attachments.
        ///     <para>
        ///         The key is the file name, and the value is content.
        ///     </para>
        /// </summary>
        public List<FileEntry> Files
        {
            get => _files ??= new List<FileEntry>(WebhookProvider.MAX_ATTACHMENTS);
        }

        #endregion

        public MessageBuilder() { }

        public MessageBuilder(MessageBuilder source)
        {
            Contract.AssertNotNull(source, nameof(source));

            Builder.Append(source.Builder.ToString());
            _username = source._username;
            AvatarUrl = source.AvatarUrl;
            IsTTS = source.IsTTS;
            _mention = source._mention;

            if (!(source._embeds is null))
                Embeds.AddRange(source._embeds);

            if (!(source._files is null))
                Files.AddRange(source._files);
        }

        #region Methods

        /// <summary>
        ///     Appends text to <see cref="Builder"/>.
        /// </summary>
        /// <returns>
        ///     The current MessageBuilder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Text is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The text exceeds <see cref="WebhookProvider.MAX_CONTENT_LENGTH" />.
        /// </exception>
        public MessageBuilder Append(string text)
        {
            Contract.AssertNotNull(text, nameof(text));
            Contract.AssertSafeBounds(Builder.Length + text.Length, -1, WebhookProvider.MAX_CONTENT_LENGTH, nameof(text));

            Builder.Append(text);

            return this;
        }

        /// <summary>
        ///     Tries to append.
        ///     Doesn't throw an exception on failure.
        /// </summary>
        /// <returns>
        ///     The current MessageBuilder.
        /// </returns>
        /// <exception cref="ArgumentNullException"><inheritdoc cref="Append(string)" /></exception>
        public MessageBuilder TryAppend(string text)
        {
            Contract.AssertNotNull(text, nameof(text));
            if (Contract.AssertSafeBoundsSafe(Builder.Length + text.Length, -1, WebhookProvider.MAX_CONTENT_LENGTH))
            {
                Builder.Append(text);
            }

            return this;
        }

        /// <summary>
        ///     Appends the <see cref="LINE_TERMINATOR"/> to the <see cref="Builder"/>.
        /// </summary>
        /// <returns><inheritdoc cref="Append(string)" /></returns>
        /// <exception cref="ArgumentOutOfRangeException"><inheritdoc cref="Append(string)" /></exception>
        public MessageBuilder AppendLine()
        {
            Contract.AssertSafeBounds(Builder.Length + LINE_TERMINATOR.Length, -1, WebhookProvider.MAX_CONTENT_LENGTH);

            Builder.Append(LINE_TERMINATOR);

            return this;
        }

        /// <summary><inheritdoc cref="TryAppend(string)" /></summary>
        /// <returns><inheritdoc cref="Append(string)" /></returns>
        public MessageBuilder TryAppendLine()
        {
            if (Contract.AssertSafeBoundsSafe(Builder.Length + LINE_TERMINATOR.Length, -1, WebhookProvider.MAX_CONTENT_LENGTH))
            {
                Builder.Append(LINE_TERMINATOR);
            }

            return this;
        }

        /// <summary>
        ///     Appends the text and the <see cref="LINE_TERMINATOR"/> to the <see cref="Builder"/>.
        /// </summary>
        /// <returns><inheritdoc cref="Append(string)" /></returns>
        /// <exception cref="ArgumentNullException"><inheritdoc cref="Append(string)" /></exception>
        /// <exception cref="ArgumentOutOfRangeException"><inheritdoc cref="Append(string)" /></exception>
        public MessageBuilder AppendLine(string text)
        {
            Contract.AssertNotNull(text, nameof(text));
            Contract.AssertSafeBounds(Builder.Length + text.Length + LINE_TERMINATOR.Length, -1, WebhookProvider.MAX_CONTENT_LENGTH, nameof(text));

            Builder.Append(text);
            Builder.Append(LINE_TERMINATOR);

            return this;
        }

        /// <summary><inheritdoc cref="TryAppendLine()" /></summary>
        /// <returns><inheritdoc cref="Append(string)" /></returns>
        /// <exception cref="ArgumentNullException"><inheritdoc cref="Append(string)" /></exception>
        public MessageBuilder TryAppendLine(string text)
        {
            Contract.AssertNotNull(text, nameof(text));
            if (Contract.AssertSafeBoundsSafe(Builder.Length + text.Length + LINE_TERMINATOR.Length, -1, WebhookProvider.MAX_CONTENT_LENGTH))
            {
                Builder.Append(text);
                Builder.Append(LINE_TERMINATOR);
            }

            return this;
        }

        /// <summary>
        ///     Adds an embed to the message.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Embed is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Exceeds the allowed limit of embeds.
        /// </exception>
        public MessageBuilder AddEmbed(IEmbed embed)
        {
            Contract.AssertNotNull(embed, nameof(embed));
            Contract.AssertSafeBounds(Embeds.Count + 1, -1, WebhookProvider.MAX_EMBED_COUNT, nameof(embed));

            Embeds.Add(embed);

            return this;
        }

        /// <summary>
        ///     Tries adds an embed to the message.
        /// </summary>
        /// <exception cref="ArgumentNullException"><inheritdoc cref="AddEmbed(IEmbed)" /></exception>
        public MessageBuilder TryAddEmbed(IEmbed embed)
        {
            Contract.AssertNotNull(embed, nameof(embed));
            if (Contract.AssertSafeBoundsSafe(Embeds.Count + 1, -1, WebhookProvider.MAX_EMBED_COUNT))
            {
                Embeds.Add(embed);
            }

            return this;
        }

        /// <summary>
        ///     Adds attachment to the message.
        /// </summary>
        /// <param name="fileEntry">
        ///     Attachment entry.
        /// </param>
        /// <returns><inheritdoc cref="Append(string)" /></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Exceeds the allowed limit of attachments.
        /// </exception>
        /// <exception cref="SizeOutOfRangeException">
        ///     Exceeds the allowed limit of attachments size.
        /// </exception>
        public MessageBuilder AddAttachment(FileEntry fileEntry)
        {
            Contract.AssertArgumentNotTrue(!fileEntry.IsValid(), nameof(fileEntry));
            Contract.AssertSafeBounds(Files.Count + 1, -1, WebhookProvider.MAX_ATTACHMENTS, nameof(fileEntry));

            Files.Add(fileEntry);
            if (!Contract.AssertNotOversizeSafe(Files))
            {
                Files.RemoveAt(Files.Count - 1);
                throw new SizeOutOfRangeException();
            }

            return this;
        }

        /// <summary>
        ///     Tries to add attachment to the message.
        /// </summary>
        /// <param name="fileEntry"><inheritdoc cref="AddAttachment(FileEntry)" /></param>
        /// <returns><inheritdoc cref="AddAttachment(FileEntry)" /></returns>
        /// <exception cref="ArgumentOutOfRangeException"><inheritdoc cref="AddAttachment(FileEntry)" /></exception>
        /// <exception cref="SizeOutOfRangeException"><inheritdoc cref="AddAttachment(FileEntry)" /></exception>
        public MessageBuilder TryAddAttachment(FileEntry fileEntry)
        {
            if (fileEntry.IsValid()
                && !Contract.AssertSafeBoundsSafe(Files.Count + 1, -1, WebhookProvider.MAX_ATTACHMENTS))
            {
                Files.Add(fileEntry);
                if (!Contract.AssertNotOversizeSafe(Files))
                    Files.RemoveAt(Files.Count - 1);
            }

            return this;
        }

        /// <summary>
        ///     Builds messages.
        /// </summary>
        public Message Build() => new Message(this);

        /// <inheritdoc />
        public void Reset()
        {
            _username = default;
            _mention = default;

            AvatarUrl = default;
            IsTTS = default;

            Builder.Clear();
            _files?.Clear();
            _embeds?.Clear();
        }

        #endregion
    }
}
