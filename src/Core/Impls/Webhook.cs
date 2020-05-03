using DSharp4Webhook.Logging;
using DSharp4Webhook.Rest;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DSharp4Webhook.Core
{
    public class Webhook : IWebhook
    {
        public ConcurrentQueue<IWebhookMessage> MessageQueue { get; } = new ConcurrentQueue<IWebhookMessage>();

        public RestClient RestClient { get; }

        private string _url;

        public IWebhookInfo WebhookInfo { get; }

        public ulong Id { get; private set; }

        public string Token { get; private set; }

        public ulong DeliveryId { get; private set; }

        public event Action<LogContext> OnLog;

        private ulong NextDeliveryId() => DeliveryId++;

        public void ResetDeileryId() => DeliveryId = 0;

        public Webhook(ulong id, string token, string url)
        {
            Id = id;
            Token = token;
            _url = url;

            DeliveryId = 0L;
            WebhookInfo = new WebhookInfo();
            RestClient = new RestClient(this);
        }

        public void Dispose()
        {
            RestClient.Dispose();
        }

        public string GetWebhookUrl()
        {
            return _url;
        }

        public void Log(LogContext context)
        {
            OnLog?.Invoke(context);
        }

        public ulong QueueMessage(IWebhookMessage message)
        {
            WebhookMessage messageImpl = new WebhookMessage(message);
            messageImpl.DeliveryId = NextDeliveryId();
            MessageQueue.Enqueue(messageImpl);
            return messageImpl.DeliveryId;
        }

        public ulong QueueMessage(string message, bool isTTS = false)
        {
            WebhookMessage messageImpl = new WebhookMessage(message, isTTS);
            messageImpl.DeliveryId = NextDeliveryId();
            MessageQueue.Enqueue(messageImpl);
            return messageImpl.DeliveryId;
        }

        public async Task SendMessage(IWebhookMessage message, bool waitForRatelimit = true)
        {
            await RestClient.SendMessage(message, waitForRatelimit);
        }

        public async Task SendMessage(string message, bool isTTS = false, bool waitForRatelimit = true)
        {
            WebhookMessage messageImpl = new WebhookMessage(message, isTTS);
            messageImpl.DeliveryId = NextDeliveryId();
            await RestClient.SendMessage(messageImpl, waitForRatelimit);
        }

        public async Task<Exception> SendMessageSafely(IWebhookMessage message, bool waitForRatelimit = true)
        {
            return await RestClient.ProcessMessage(message, waitForRatelimit);
        }
    }
}
