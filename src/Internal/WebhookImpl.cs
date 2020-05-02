using DSharp4Webhook.Core;
using DSharp4Webhook.Entities;
using DSharp4Webhook.Logging;
using DSharp4Webhook.Rest;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DSharp4Webhook.Internal
{
    internal class WebhookImpl : IWebhook
    {
        public ConcurrentQueue<IWebhookMessage> MessageQueue { get; } = new ConcurrentQueue<IWebhookMessage>();

        public RestClient RestClient { get; }

        private string _url;

        public IBaseWebhookData WebhookData { get; } = new WebhookDataImpl();

        public ulong Id { get; private set; } = 0;

        public string Token { get; private set; } = null;

        public ulong DeliveryId { get; private set; } = 0;

        public event Action<LogContext> OnLog;

        private ulong NextDeliveryId() => DeliveryId++;

        public void ResetDeileryId() => DeliveryId = 0;

        public WebhookImpl(ulong id, string token, string url)
        {
            Id = id;
            Token = token;
            _url = url;

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
            WebhookMessageImpl messageImpl = new WebhookMessageImpl(message);
            messageImpl.DeliveryId = NextDeliveryId();
            MessageQueue.Enqueue(messageImpl);
            return messageImpl.DeliveryId;
        }

        public ulong QueueMessage(string message, bool isTTS = false)
        {
            WebhookMessageImpl messageImpl = new WebhookMessageImpl(message, isTTS);
            messageImpl.DeliveryId = NextDeliveryId();
            MessageQueue.Enqueue(messageImpl);
            return messageImpl.DeliveryId;
        }

        public async Task SendMessage(IWebhookMessage message, bool waitForRatelimit = false)
        {
            await RestClient.SendMessage(message.DeliveryId, message, waitForRatelimit);
        }

        public async Task SendMessage(string message, bool isTTS = false, bool waitForRatelimit = false)
        {
            WebhookMessageImpl messageImpl = new WebhookMessageImpl(message, isTTS);
            messageImpl.DeliveryId = NextDeliveryId();
            await RestClient.SendMessage(messageImpl.DeliveryId, messageImpl, waitForRatelimit);
        }

        public async Task<Exception> SendMessageSafely(IWebhookMessage message, bool waitForRatelimit = false)
        {
            return await RestClient.ProcessMessage(message.DeliveryId, message, waitForRatelimit);
        }
    }
}
