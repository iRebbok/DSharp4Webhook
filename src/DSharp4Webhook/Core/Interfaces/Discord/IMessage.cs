using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Serialization;
using System;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     The webhook message.
    /// </summary>
    public interface IMessage : INetSerializable
    {
        /// <summary>
        ///     Nickname of the webhook that will be displayed.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     When you try to set <see cref="string.Empty"/> or a value greater than 80 characters.
        /// </exception>
        public string? Username { get; }

        /// <summary>
        ///     Avatar that will be displayed in webhook with the message.
        /// </summary>
        public string? AvatarUrl { get; }

        /// <summary>
        ///     The content of the message.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     When you try to set a value greater than 2000 characters.
        /// </exception>
        public string? Content { get; }

        /// <summary>
        ///     Whether the TTS determines this message or not.
        /// </summary>        
        public bool IsTTS { get; }

        /// <summary>
        ///     Message embeds.
        /// </summary>
        public IEmbed[]? Embeds { get; }

        /// <summary>
        ///     Allowed mentions for a message.
        /// </summary>
        public IMessageMention Mention { get; }

        /// <summary>
        ///     Attachments to the message.
        /// </summary>
        public FileEntry[]? Attachments { get; }
    }
}
