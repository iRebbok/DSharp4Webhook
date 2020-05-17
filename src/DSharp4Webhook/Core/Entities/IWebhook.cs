using DSharp4Webhook.Action;
using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Internal;
using DSharp4Webhook.Rest;
using DSharp4Webhook.Rest.Manipulation;
using System;

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
        WebhookProvider? Provider { get; }

        /// <summary>
        ///     Provider for REST requests.
        /// </summary>
        BaseRestProvider RestProvider { get; }

        /// <summary>
        ///     Action manager.
        /// </summary>
        IActionManager ActionManager { get; }

        /// <summary>
        ///     Webhook statuses.
        ///     Used for internal processing of rest requests.
        ///     Do not set the values without the need.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     When trying to assign a value to a nonexistent webhook
        ///     or downgrade the status to an existing webhook.
        /// </exception>
        WebhookStatus Status { get; set; }

        /// <summary>
        ///     Rest settings that will be used when creating rest queries.
        /// </summary>
        RestSettings RestSettings { get; set; }

        /// <summary>
        ///     Webhook id.
        /// </summary>
        ulong Id { get; }

        /// <summary>
        ///     Webhook token.
        /// </summary>
        string Token { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the url of the webhack to interact with the API,
        ///     and your subdomain Url can be used if it is valid.
        /// </summary>
        string GetWebhookUrl();

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
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
        IMessageAction SendMessage(string message, bool isTTS = false, RestSettings? restSettings = null);

        /// <summary>
        ///     Send messages.
        /// </summary>
        /// <param name="message">
        ///     A message that can be build via MessageBuilder.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
        IMessageAction SendMessage(IMessage message, RestSettings? restSettings = null);

        /// <summary>
        ///     Retrieves information about webhook.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
        IInfoAction GetInfo(RestSettings? restSettings = null);

        /// <summary>
        ///     Deletes the webhook.
        ///     Destroys webhook at the level of the discord.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
        IDeleteAction Delete(RestSettings? restSettings = null);

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
        IModifyAction Modify(string name, RestSettings? restSettings = null);

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
        IModifyAction Modify(string name, IWebhookImage image, RestSettings? restSettings = null);

        #endregion
    }
}
