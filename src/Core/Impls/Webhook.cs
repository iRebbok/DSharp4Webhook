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

        public IWebhookInfo WebhookInfo { get; }

        public ulong Id { get => _id; }
        public string Token { get => _token; }

        public event Action<LogContext> OnLog;

        private readonly string _url;
        private readonly ulong _id;
        private readonly string _token;

        public Webhook(ulong id, string token, string url)
        {
            _id = id;
            _token = token;
            _url = url;

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

        public void QueueMessage(IWebhookMessage message)
        {
            MessageQueue.Enqueue(message);
        }

        public void QueueMessage(string message, bool isTTS = false)
        {
            WebhookMessage messageImpl = new WebhookMessage(message, isTTS);
            MessageQueue.Enqueue(messageImpl);
        }

        public async Task SendMessageAsync(IWebhookMessage message)
        {
            await RestClient.SendMessage(message);
        }

        public async Task SendMessageAsync(string message, bool isTTS = false)
        {
            WebhookMessage messageImpl = new WebhookMessage(message, isTTS);
            await RestClient.SendMessage(messageImpl);
        }

        public async Task<Exception> SendMessageAsyncSafely(IWebhookMessage message)
        {
            return await RestClient.ProcessMessage(message);
        }
    }
}
