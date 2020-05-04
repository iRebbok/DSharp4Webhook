using DSharp4Webhook.Rest;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DSharp4Webhook.Core
{
    public class Webhook : IWebhook
    {
        public ConcurrentQueue<IWebhookMessage> MessageQueue { get => _queue; }
        public RestClient RestClient { get => _restClient; }
        public IWebhookInfo WebhookInfo { get => _webhookInfo; }
        public WebhookProvider Provider { get => _provider; }
        public ulong Id { get => _id; }
        public string Token { get => _token; }

        private readonly ConcurrentQueue<IWebhookMessage> _queue;
        private readonly WebhookProvider _provider;
        private readonly RestClient _restClient;
        private readonly IWebhookInfo _webhookInfo;

        private readonly ulong _id;
        private readonly string _token;
        private readonly string _url;

        public Webhook(WebhookProvider provider, ulong id, string token, string url)
        {
            _id = id;
            _token = token;
            _url = url;

            _queue = new ConcurrentQueue<IWebhookMessage>();
            _webhookInfo = new WebhookInfo();
            _restClient = new RestClient(this);
            _provider = provider;
        }

        public void Dispose()
        {
            RestClient.Dispose();
        }

        public string GetWebhookUrl()
        {
            return _url;
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

        public async Task<Exception> SendMessageAsyncSafely(string message, bool isTTS = false)
        {
            WebhookMessage messageImpl = new WebhookMessage(message, isTTS);
            return await RestClient.ProcessMessage(messageImpl);
        }
    }
}
