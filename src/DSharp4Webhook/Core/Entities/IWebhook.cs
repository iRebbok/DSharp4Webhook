using DSharp4Webhook.Action;
using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Internal;
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
        IMessageAction SendMessage(string message, bool isTTS = false);

        /// <summary>
        ///     Send messages.
        /// </summary>
        /// <param name="message">
        ///     A message that can be build via MessageBuilder.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
        IMessageAction SendMessage(IMessage message);

        /// <summary>
        ///     Retrieves information about webhook.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
        IInfoAction GetInfo();

        /// <summary>
        ///     Deletes the webhook.
        ///     Destroys webhook at the level of the discord.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
        IDeleteAction Delete();

        /// <summary>
        ///     Updates the webhook.
        /// </summary>
        /// <param name="name">
        ///     Webhook name.
        /// </param>
        /// <param name="image">
        ///     Avatar that will use webhook.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     One of the arguments does not meet the requirements.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     When trying to interact with a nonexistent webhook.
        /// </exception>
        IUpdateAction Modify(string name, IWebhookImage image);

        #endregion
    }
}
