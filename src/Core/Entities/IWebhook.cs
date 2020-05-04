using DSharp4Webhook.Logging;
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
    public interface IWebhook : IDisposable, ILogable
    {
        #region Properties

        /// <summary>
        ///     Message queue to send.
        /// </summary>
        ConcurrentQueue<IWebhookMessage> MessageQueue { get; }

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
        /// <returns>
        ///     Delivery id of this message.
        /// </returns>
        void QueueMessage(IWebhookMessage message);

        /// <summary>
        ///     Sends a message to the queue.
        /// </summary>
        /// <param name="message">
        ///     Message as content.
        /// </param>
        /// <param name="isTTS">
        ///     Whether the TTS determines this message or not.
        /// </param>
        /// <returns>
        ///     Delivery id of this message.
        /// </returns>
        void QueueMessage(string message, bool isTTS = false);

        /// <summary>
        ///     Sends a message synchronously blocking the main thread.
        /// </summary>
        /// <param name="message">
        ///     A message that can be built via MessageBuilder.
        /// </param>
        /// <param name="waitForRatelimit">
        ///     Defines waiting for the rate limit, 
        ///     if true, it waits until the rate limit ends, 
        ///     delaying the main thread all the time.
        /// </param>
        Task SendMessage(IWebhookMessage message, bool waitForRatelimit = true);

        /// <summary>
        ///     Sends a message synchronously blocking the main thread.
        /// </summary>
        /// <param name="message">
        ///     Message as content.
        /// </param>
        /// <param name="isTTS">
        ///     Whether the TTS determines this message or not.
        /// </param>
        /// <param name="waitForRatelimit">
        ///     Defines waiting for the rate limit, 
        ///     if true, it waits until the rate limit ends, 
        ///     delaying the main thread all the time.
        /// </param>
        Task SendMessage(string message, bool isTTS = false, bool waitForRatelimit = true);

        /// <summary>
        ///     Sends a message synchronously blocking the main thread.
        ///     The only difference is that it does not throw an exception if it occurs, 
        ///     but returns it as a variable.
        /// </summary>
        /// <param name="message">
        ///     A message that can be built via MessageBuilder.
        /// </param>
        /// <param name="waitForRatelimit">
        ///     Defines waiting for the rate limit, 
        ///     if true, it waits until the rate limit ends, 
        ///     delaying the main thread all the time. 
        /// </param>
        Task<Exception> SendMessageSafely(IWebhookMessage message, bool waitForRatelimit = true);

        #endregion
    }
}
