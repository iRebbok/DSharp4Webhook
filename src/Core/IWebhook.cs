using DSharp4Webhook.Entities;
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
    ///     The implementation is located in <see cref="WebhookImpl"/>.
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
        IBaseWebhookData WebhookData { get; }

        /// <summary>
        ///     Webhook id.
        /// </summary>
        ulong Id { get; }

        /// <summary>
        ///     Webhook token.
        /// </summary>
        string Token { get; }

        /// <summary>
        ///     Gets the delivery id of the future message.
        /// </summary>
        ulong DeliveryId { get; }

        #endregion

        #region Methods

        /// <summary>
        ///     Resets the id of the delivery.
        /// </summary>
        void ResetDeileryId();

        /// <summary>
        ///     Gets the webhook Url or null if its data is incorrect.
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
        ulong QueueMessage(IWebhookMessage message);

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
        ulong QueueMessage(string message, bool isTTS = false);

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
        Task SendMessage(IWebhookMessage message, bool waitForRatelimit = false);

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
        Task SendMessage(string message, bool isTTS = false, bool waitForRatelimit = false);

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
        Task<Exception> SendMessageSafely(IWebhookMessage message, bool waitForRatelimit = false);

        #endregion
    }
}
