using DSharp4Webhook.Entities;
using DSharp4Webhook.Internal;
using System;
using System.Text;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     Message builder that allows you to create messages for webhook.
    /// </summary>
    public class MessageBuilder : IDisposable
    {
        public static MessageBuilder New => new MessageBuilder();

        private StringBuilder builder = new StringBuilder();
        private string avatarUrl;
        private string username;
        private bool isTTS;

        private MessageBuilder() { }

        /// <summary>
        ///     Adds text to the current text.
        /// </summary>
        /// <returns>
        ///     True if successful, false if not
        /// </returns>
        public bool Append(string text)
        {
            if (builder.Length + text.Length < 2000)
            {
                builder.Append(text);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Adds a new line to the current text.
        /// </summary>
        /// <returns>
        ///     True if successful, false if not
        /// </returns>
        public bool AppendLine()
        {
            if (builder.Length + 1 < 2000)
            {
                builder.AppendLine();
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Adds text to the current text in a new line.
        /// </summary>
        /// <returns>
        ///     True if successful, false if not
        /// </returns>
        public bool AppendLine(string text)
        {
            if (builder.Length + text.Length < 2000)
            {
                builder.AppendLine(text);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Resets the current text and sets
        /// </summary>
        /// <returns>
        ///     True if successful, false if not
        /// </returns>
        public bool Set(string text)
        {
            if (text.Length < 2000)
            {
                builder.Clear();
                builder.Append(text);
                return true;
            }
            return false;
        }

        public void SetAvatarUrl(string url)
        {
            avatarUrl = url;
        }

        public string GetAvatarUrl()
        {
            return avatarUrl;
        }

        public bool SetUsername(string name)
        {
            if (name.Length <= 80)
            {
                username = name;
                return true;
            }
            return false;
        }

        public string GetUsername()
        {
            return username;
        }

        public void SetTTS(bool value)
        {
            isTTS = value;
        }

        public bool GetTTS()
        {
            return isTTS;
        }

        public IWebhookMessage Build()
        {
            IWebhookMessage message = new WebhookMessageImpl(builder.ToString(), isTTS);
            message.AvatarUrl = avatarUrl;
            message.Username = username;
            return message;
        }

        public void Dispose()
        {
            builder.Clear();
            avatarUrl = null;
            username = null;
        }
    }
}
