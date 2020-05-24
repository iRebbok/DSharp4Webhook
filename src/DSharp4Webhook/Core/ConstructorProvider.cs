using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Internal;
using System;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     Provides various constructors
    ///     for interacting with a webhook.
    /// </summary>
    public static class ConstructorProvider
    {
        /// <summary>
        ///     Gets default mentions in the message.
        /// </summary>
        public static IMessageMention GetDefaultMessageMention() => new MessageMention(AllowedMention.NONE);

        /// <summary>
        ///     Gets the specified message metinon.
        /// </summary>
        /// <param name="mention">
        ///     Mentions that will be allowed.
        /// </param>
        public static IMessageMention GetMessageMention(AllowedMention mention) => new MessageMention(mention);

        /// <summary>
        ///     Gets a new message constructor.
        /// </summary>
        public static MessageBuilder GetMessageBuilder() => MessageBuilder.New();

        /// <summary>
        ///     Gets a new message constructor with source presets.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="source"/> is null
        /// </exception>
        public static MessageBuilder GetMessageBuilder(MessageBuilder source) => MessageBuilder.New(source);

        /// <summary>
        ///     Gets a new message constructor with a preset of allowed mentions from the webhook.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="webhook"/> is null.
        /// </exception>
        public static MessageBuilder GetMessageBuilder(IWebhook webhook) => MessageBuilder.New(webhook);

        /// <summary>
        ///     Gets a new modifier content constructor.
        /// </summary>
        /// <returns></returns>
        public static ModifyContentBuilder GetModifyContentBuilder() => ModifyContentBuilder.New();
    }
}
