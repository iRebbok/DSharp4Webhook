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
        private readonly Message _message;
        private readonly StringBuilder _builder;

        #region Properties

        /// <summary>
        ///     The content of the message.
        /// </summary>
        public string Content
        {
            get => _message.Content;
            set => _message.Content = value;
        }

        /// <summary>
        ///     Whether the TTS determines this message or not.
        /// </summary>
        public bool IsTTS
        {
            get => _message.IsTTS;
            set => _message.IsTTS = value;
        }

        /// <summary>
        ///     Username of the webhook that will be used for this message.
        /// </summary>
        public string Username
        {
            get => _message.Username;
            set => _message.Username = value;
        }

        /// <summary>
        ///     An image that will use webhook on this message.
        /// </summary>
        public string AvatarUrl
        {
            get => _message.AvatarUrl;
            set => _message.AvatarUrl = value;
        }

        #endregion

        #region Static methods

        /// <summary>
        ///     Gets default mentions in the message.
        /// </summary>
        public static IMessageMention GetDefaultMessageMention() => new MessageMention(AllowedMention.NONE);

        #endregion

        public MessageBuilder()
        {
            _builder = new StringBuilder();
            _message = new Message();
        }

        public MessageBuilder(MessageBuilder source)
        {
            _message = source._message;
            _builder = source._builder;
        }

        #region Methods

        /// <summary>
        ///     Adds text to the current text.
        /// </summary>
        /// <returns>
        ///     The current MessageBuilder.
        /// </returns>
        public MessageBuilder Append(string text)
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
        public MessageBuilder TryAppend(string text)
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
        public MessageBuilder AppendLine(string text)
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
        public MessageBuilder TryAppendLine(string text)
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
            _message.Mention = messageMention;

            return this;
        }

        public IMessage Build()
        {
            _message.Content = _builder.ToString();
            return _message;
        }

        public void Dispose()
        {
            _builder.Clear();

            // a null value does not cause an error, just clears it
            _message.Mention.Roles = null;
            _message.Mention.Users = null;
        }

        #endregion
    }
}
