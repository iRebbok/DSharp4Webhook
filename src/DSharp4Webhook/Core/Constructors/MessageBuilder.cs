using DSharp4Webhook.Internal;
using DSharp4Webhook.Util;
using System;
using System.Text;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     Message builder that allows you to create messages for webhook.
    /// </summary>
    public sealed class MessageBuilder : IDisposable
    {
        private readonly StringBuilder _builder;

        private string _username;
        private string _avatarUrl;
        private bool _isTTS;
        private IMessageMention _mention;

        #region Properties

        /// <summary>
        ///     Gets the message builder for this builder.
        /// </summary>
        public StringBuilder Builder { get => _builder; }

        /// <summary>
        ///     Whether the TTS determines this message or not.
        /// </summary>
        public bool IsTTS
        {
            get => _isTTS;
            set => _isTTS = value;
        }

        /// <summary>
        ///     Username of the webhook that will be used for this message.
        /// </summary>
        public string Username
        {
            get => _username;
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                    Checks.CheckBounds(nameof(Username), $"Must be between {WebhookProvider.MIN_NICKNAME_LENGTH} and {WebhookProvider.MAX_NICKNAME_LENGTH} in length.", WebhookProvider.MAX_NICKNAME_LENGTH + 1, value.Length);
                    Checks.CheckBoundsUnderside(nameof(Username), $"Must be between {WebhookProvider.MIN_NICKNAME_LENGTH} and {WebhookProvider.MAX_NICKNAME_LENGTH} in length.",
                        WebhookProvider.MIN_NICKNAME_LENGTH - 1, value.Length);
                    _username = value;
                }
                // Null set possible
                else
                    _username = value;
            }
        }

        /// <summary>
        ///     An image that will use webhook on this message.
        /// </summary>
        public string AvatarUrl
        {
            get => _avatarUrl;
            set => _avatarUrl = value;
        }

        /// <summary>
        ///     Allowed mentions in the message.
        /// </summary>
        public IMessageMention MessageMention
        {
            get => _mention;
            set => _mention = value ?? _mention;
        }

        #endregion

        private MessageBuilder()
        {
            _builder = new StringBuilder();
        }

        private MessageBuilder(MessageBuilder source) : this()
        {
            Checks.CheckForNull(source, nameof(source));

            _builder.Append(source._builder.ToString());
            _username = source._username;
            _avatarUrl = source._avatarUrl;
            _isTTS = source._isTTS;
        }

        private MessageBuilder(IWebhook webhook) : this()
        {
            Checks.CheckForNull(webhook, nameof(webhook));

            _mention = ConstructorProvider.GetMessageMention(webhook.AllowedMention);
        }

        #region Static methods

        /// <summary>
        ///     Gets a new message constructor.
        /// </summary>
        public static MessageBuilder New() => new MessageBuilder();

        /// <summary>
        ///     Gets a new message constructor with source presets.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="source"/> is null
        /// </exception>
        public static MessageBuilder New(MessageBuilder source) => new MessageBuilder(source);

        /// <summary>
        ///     Gets a new message constructor with a preset of allowed mentions from the webhook.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="webhook"/> is null.
        /// </exception>
        public static MessageBuilder New(IWebhook webhook) => new MessageBuilder(webhook);

        #endregion

        #region Methods

        /// <summary>
        ///     Adds text to the current text.
        /// </summary>
        /// <returns>
        ///     The current MessageBuilder.
        /// </returns>
#nullable enable
        public MessageBuilder Append(string? text)
#nullable restore
        {
            // If we put null, it will still be null in the text
            Checks.CheckBounds(nameof(text), $"The text cannot exceed the {WebhookProvider.MAX_CONTENT_LENGTH} character limit",
                WebhookProvider.MAX_CONTENT_LENGTH, text?.Length ?? 4, _builder.Length);
            _builder.Append(text ?? "null");

            return this;
        }

        /// <summary>
        ///     Tries to add text, 
        ///     without causing an exception when the bounds are exceeded.
        /// </summary>
        /// <returns>
        ///     The current MessageBuilder.
        /// </returns>
#nullable enable
        public MessageBuilder TryAppend(string? text)
#nullable restore
        {
            if (!Checks.CheckBoundsSafe(WebhookProvider.MAX_CONTENT_LENGTH, text?.Length ?? 4, _builder.Length))
                _builder.Append(text ?? "null");

            return this;
        }

        /// <summary>
        ///     Adds a new line to the current text.
        /// </summary>
        /// <returns>
        ///     The current MessageBuilder.
        /// </returns>
        public MessageBuilder AppendLine()
        {
            Checks.CheckBounds(string.Empty, $"The text cannot exceed the {WebhookProvider.MAX_CONTENT_LENGTH} character limit",
                WebhookProvider.MAX_CONTENT_LENGTH, 1, _builder.Length);
            _builder.AppendLine();

            return this;
        }

        /// <summary>
        ///     Tries to add a new line to the current text,
        ///     without causing an exception when the bounds are exceeded.
        /// </summary>
        /// <returns>
        ///     The current MessageBuilder.
        /// </returns>
        public MessageBuilder TryAppendLine()
        {
            if (!Checks.CheckBoundsSafe(WebhookProvider.MAX_CONTENT_LENGTH, 1, _builder.Length))
                _builder.AppendLine();

            return this;
        }

        /// <summary>
        ///     Adds text to the current text in a new line.
        /// </summary>
        /// <returns>
        ///     The current MessageBuilder.
        /// </returns>
#nullable enable
        public MessageBuilder AppendLine(string? text)
#nullable restore
        {
            // If we put null, it will still be null in the text, a line break is also added
            Checks.CheckBounds(nameof(text), $"The text cannot exceed the {WebhookProvider.MAX_CONTENT_LENGTH} character limit",
                WebhookProvider.MAX_CONTENT_LENGTH, (text?.Length ?? 4) + 1, _builder.Length);
            _builder.AppendLine(text ?? "null");

            return this;
        }

        /// <summary>
        ///     Tries to add text to the current text in a new line,
        ///     without causing an exception when the bounds are exceeded.
        /// </summary>
        /// <returns>
        ///     The current MessageBuilder.
        /// </returns>
#nullable enable
        public MessageBuilder TryAppendLine(string? text)
#nullable restore
        {
            if (!Checks.CheckBoundsSafe(WebhookProvider.MAX_CONTENT_LENGTH, text?.Length ?? 4 + 1, _builder.Length))
                _builder.AppendLine(text ?? "null");

            return this;
        }

        /// <summary>
        ///     Sets the handler for mentions in the message.
        /// </summary>
        /// <returns>
        ///     The current MessageBuilder.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="messageMention"/> is null.
        /// </exception>
        public MessageBuilder SetMessageMention(IMessageMention messageMention)
        {
            Checks.CheckForNull(messageMention, nameof(messageMention));
            _mention = messageMention;

            return this;
        }

        /// <summary>
        ///     Builds messages.
        /// </summary>
        public IMessage Build()
        {
            return new Message(this);
        }

        /// <summary>
        ///     Resets the entire preset, but not allowed mentions.
        ///     It can be used to reload the constructor.
        /// </summary>
        public void Dispose()
        {
            _builder.Clear();
            _avatarUrl = null;
            _isTTS = false;
            _username = null;
        }

        #endregion
    }
}
