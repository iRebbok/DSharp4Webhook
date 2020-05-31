using DSharp4Webhook.Action;
using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Internal;
using DSharp4Webhook.Rest;
using DSharp4Webhook.Rest.Manipulation;
using System;
using System.Collections.Generic;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     Basic webhook.
    /// </summary>
    /// <remarks>
    ///     The implementation is located in <see cref="Webhook"/>.
    /// </remarks>
    public interface IWebhook : IDisposable
    {
        #region Properties

        /// <summary>
        ///     Webhook provider.
        ///     Is null for webhooks created without a provider.
        /// </summary>
#nullable enable
        public WebhookProvider? Provider { get; }
#nullable restore

        /// <summary>
        ///     Provider for REST requests.
        /// </summary>
        public BaseRestProvider RestProvider { get; }

        /// <summary>
        ///     Action manager.
        /// </summary>
        public IActionManager ActionManager { get; }

        /// <summary>
        ///     Webhook statuses.
        ///     Used for internal processing of rest requests.
        ///     Do not set the values without the need.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     When trying to assign a value to a nonexistent webhook
        ///     or downgrade the status to an existing webhook.
        /// </exception>
        public WebhookStatus Status { get; set; }

        /// <summary>
        ///     Rest settings that will be used when creating rest queries.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Attempt to set a null value.
        /// </exception>
        public RestSettings RestSettings { get; set; }

        /// <summary>
        ///     Allowed mentions for webhook.
        /// </summary>
        public AllowedMention AllowedMention { get; set; }

        /// <summary>
        ///     Webhook id.
        /// </summary>
        public ulong Id { get; }

        /// <summary>
        ///     Webhook token.
        /// </summary>
        public string Token { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the url of the webhack to interact with the API,
        ///     and your subdomain Url can be used if it is valid.
        /// </summary>
        public string GetWebhookUrl();

        /// <summary>
        ///     Send messages.
        /// </summary>
        /// <param name="message">
        ///     Message content.
        /// </param>
        /// <param name="isTTS">
        ///     Manages the voice over of the message to all clients
        ///     who are in the corresponding channel.
        /// </param>
        /// <param name="messageMention">
        ///     Settings for allowed mentions.
        ///     By default, the current value for the webhook is used.
        /// </param>
        /// <param name="restSettings">
        ///     Settings for rest request.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     The message is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The message exceeds the allowed length on <see cref="WebhookProvider.MAX_CONTENT_LENGTH"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
#nullable enable
        public IMessageAction SendMessage(string message, bool isTTS = false, IMessageMention? messageMention = null, RestSettings? restSettings = null);
#nullable restore

        /// <summary>
        ///     Send messages.
        /// </summary>
        /// <param name="message">
        ///     A message that can be build via MessageBuilder.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
#nullable enable
        public IMessageAction SendMessage(IMessage message, RestSettings? restSettings = null);
#nullable restore

        /// <summary>
        ///     Send messages.
        /// </summary>
        /// <param name="embeds">
        ///     List with embeds.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     List is empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     List with embeds is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     List contains more than is allowed on <see cref="WebhookProvider.MAX_EMBED_COUNT"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
#nullable enable
        public IMessageAction SendMessage(IEnumerable<IEmbed> embeds, IMessageMention? messageMention = null, RestSettings? restSettings = null);
#nullable restore

        /// <summary>
        ///     Send messages.
        /// </summary>
        /// <param name="embed">
        ///     One embed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Embed is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
#nullable enable
        public IMessageAction SendMessage(IEmbed embed, IMessageMention? messageMention = null, RestSettings? restSettings = null);
#nullable restore

        /// <summary>
        ///     Retrieves information about webhook.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
#nullable enable
        public IInfoAction GetInfo(RestSettings? restSettings = null);
#nullable restore

        /// <summary>
        ///     Deletes the webhook.
        ///     Destroys webhook at the level of the discord.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
#nullable enable
        public IDeleteAction Delete(RestSettings? restSettings = null);
#nullable restore

        /// <summary>
        ///     Modifies the webhook.
        ///     <para>
        ///         The only difference from a similar method is that it does not modify the image.
        ///     </para>
        /// </summary>
        /// <param name="name">
        ///     Webhook name.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     One of the arguments does not meet the requirements.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
#nullable enable
        public IModifyAction Modify(string name, RestSettings? restSettings = null);
#nullable restore

        /// <summary>
        ///     Modifies the webhook.
        /// </summary>
        /// <param name="name">
        ///     Webhook name.
        /// </param>
        /// <param name="image">
        ///     Avatar that will use webhook.
        ///     A null value will mean resetting the image for the webhook.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     One of the arguments does not meet the requirements.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
#nullable enable
        public IModifyAction Modify(string name, IWebhookImage image, RestSettings? restSettings = null);
#nullable restore

        /// <summary>
        ///     Modifies the webhook using pre-prepared data.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="content"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
#nullable enable
        public IModifyAction Modify(IModifyContent content, RestSettings? restSettings = null);
#nullable restore

        #endregion
    }
}
