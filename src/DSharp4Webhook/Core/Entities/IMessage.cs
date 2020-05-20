using DSharp4Webhook.Serialization;
using System;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     The webhook message.
    /// </summary>
    public interface IMessage : IWSerializable
    {
        /// <summary>
        ///     Nickname of the webhook that will be displayed.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     When you try to set <see cref="string.Empty"/> or a value greater than 80 characters.
        /// </exception>
#nullable enable
        public string? Username { get; }
#nullable restore

        /// <summary>
        ///     Avatar that will be displayed in webhook with the message.
        /// </summary>
#nullable enable
        public string? AvatarUrl { get; }
#nullable restore

        /// <summary>
        ///     The content of the message.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     When you try to set a value greater than 2000 characters.
        /// </exception>
#nullable enable
        public string? Content { get; }
#nullable restore

        /// <summary>
        ///     Whether the TTS determines this message or not.
        /// </summary>        
        public bool IsTTS { get; }

        /// <summary>
        ///     Allowed mentions for a message.
        /// </summary>
        public IMessageMention Mention { get; }
    }
}
