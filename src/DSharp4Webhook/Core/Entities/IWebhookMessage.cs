
using DSharp4Webhook.Serialization;
using System;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     The webhook message.
    /// </summary>
    public interface IWebhookMessage : IWebhookMessageInfo, IWSerializable
    {
        /// <summary>
        ///     The content of the message.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     When you try to set a value greater than 2000 characters.
        /// </exception>
#nullable enable
        string Content { get; set; }

        /// <summary>
        ///     Whether the TTS determines this message or not.
        /// </summary>        
        bool IsTTS { get; set; }
    }
}
