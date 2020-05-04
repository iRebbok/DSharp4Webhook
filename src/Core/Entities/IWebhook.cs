using DSharp4Webhook.Rest;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

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
        ///     Message queue to send.
        /// </summary>
        ConcurrentQueue<IWebhookMessage> MessageQueue { get; }

        /// <summary>
        ///     Webhook provider.
        ///     Is null for webhooks created without a provider.
        /// </summary>
        WebhookProvider Provider { get; }

        /// <summary>
        ///     Used by RestClient to send data.
        /// </summary>
        RestClient RestClient { get; }

        /// <summary>
        ///     Constant data that is used when sending a message.
        /// </summary>
        IWebhookInfo WebhookInfo { get; }

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
        ///     Sends a message to the queue.
        /// </summary>
        /// <param name="message">
        ///     A message that can be built via MessageBuilder.
        /// </param>
        void QueueMessage(IWebhookMessage message);

        /// <summary>
        ///     Sends a message to the queue.
        /// </summary>
        /// <param name="message">
        ///     Message content.
        /// </param>
        /// <param name="isTTS">
        ///     Manages the voice over of the message to all clients
        ///     who are in the corresponding channel.
        /// </param>
        void QueueMessage(string message, bool isTTS = false);

        /// <summary>
        ///     Sends a message asynchronously out of a queue.
        /// </summary>
        /// <param name="message">
        ///     A message that can be build via MessageBuilder.
        /// </param>
        Task SendMessageAsync(IWebhookMessage message);

        /// <summary>
        ///     Sends a message asynchronously out of a queue.
        /// </summary>
        /// <param name="message">
        ///     Message content.
        /// </param>
        /// <param name="isTTS">
        ///     Manages the voice over of the message to all clients
        ///     who are in the corresponding channel.
        /// </param>
        Task SendMessageAsync(string message, bool isTTS = false);

        /// <summary>
        ///     Sends a message asynchronously out of a queue,
        ///     The only difference is that it does not throw an exception if it occurs, 
        ///     but returns it as a variable.
        /// </summary>
        /// <param name="message">
        ///     A message that can be built via MessageBuilder.
        /// </param>
        Task<Exception> SendMessageAsyncSafely(IWebhookMessage message);

        /// <summary>
        ///     Sends a message asynchronously out of a queue,
        ///     The only difference is that it does not throw an exception if it occurs, 
        ///     but returns it as a variable.
        /// </summary>
        /// <param name="mesage">
        ///     A message that can be build via MessageBuilder.
        /// </param>
        /// <param name="isTTS">
        ///     Manages the voice over of the message to all clients
        ///     who are in the corresponding channel.
        /// </param>
        /// <returns></returns>
        Task<Exception> SendMessageAsyncSafely(string mesage, bool isTTS = false);

        #endregion
    }
}
