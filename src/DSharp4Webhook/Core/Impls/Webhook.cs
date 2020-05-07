using DSharp4Webhook.Rest;
using DSharp4Webhook.Util;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DSharp4Webhook.Core
{
    public class Webhook : IWebhook
    {
        public ConcurrentQueue<IWebhookMessage> MessageQueue { get => _queue; }
        public RestClient RestClient { get => _restClient; }
        public IWebhookMessageInfo WebhookMessageInfo { get => _webhookInfo; }
        public WebhookProvider Provider { get => _provider; }
        public WebhookStatus Status
        {
            get => _status;
            set
            {
                // Don't allow to change the status for nonexistent webhooks
                if (_status == WebhookStatus.NOT_EXISTING)
                    throw new InvalidOperationException("Attempt to assign a third-party status to a nonexistent webhook");
                // Don't allow to downgrade the status for existing webhooks
                if (_status == WebhookStatus.EXISTING && value == WebhookStatus.NOT_CHECKED)
                    throw new InvalidOperationException("Attempt to downgrade the status of an existing web hook");

                _status = value;
            }
        }
        public ulong Id { get => _id; }
        public string Token { get => _token; }

        private readonly ConcurrentQueue<IWebhookMessage> _queue;
        private readonly WebhookProvider _provider;
        private readonly RestClient _restClient;
        private readonly IWebhookMessageInfo _webhookInfo;
        private WebhookStatus _status;

        private readonly ulong _id;
        private readonly string _token;
        private readonly string _url;

        /// <summary>
        ///     Creates a webhook.
        /// </summary>
        /// <param name="provider">
        ///     Provider of the webhook, may be null.
        /// </param>
        /// <param name="id">
        ///     Webhook id.
        /// </param>
        /// <param name="token">
        ///     Webhook token.
        /// </param>
        /// <param name="url">
        ///     Webhook url.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If the url or token is null or empty.
        /// </exception>
        public Webhook(WebhookProvider provider, ulong id, string token, string url)
        {
            Checks.CheckForArgument(string.IsNullOrEmpty(token), nameof(token), "The token can't be empty or null");
            Checks.CheckForArgument(string.IsNullOrEmpty(url), nameof(url), "The url can't be empty or null");

            _id = id;
            _token = token;
            _url = url;

            _queue = new ConcurrentQueue<IWebhookMessage>();
            _webhookInfo = new WebhookMessageInfo();
            _restClient = new RestClient(this);
            _provider = provider;
            // Setting the unverified status
            _status = WebhookStatus.NOT_CHECKED;
        }

        public void Dispose()
        {
            // just take out the values until they run out
            while (_queue.TryDequeue(out _)) { }

            RestClient.Dispose();
        }

        public string GetWebhookUrl()
        {
            return _url;
        }

        public void QueueMessage(IWebhookMessage message)
        {
            Checks.CheckWebhookStatus(_status);
            Checks.CheckForNull(message, nameof(message), "The message cannot be null");
            MessageQueue.Enqueue(message);
        }

        public void QueueMessage(string message, bool isTTS = false)
        {
            Checks.CheckWebhookStatus(_status);
            Checks.CheckForArgument(string.IsNullOrWhiteSpace(message), nameof(message), "The message cannot be empty, null, or completely whitespace");
            WebhookMessage messageImpl = new WebhookMessage(message, isTTS);
            MessageQueue.Enqueue(messageImpl);
        }

        public async Task SendMessageAsync(IWebhookMessage message)
        {
            Checks.CheckWebhookStatus(_status);
            Checks.CheckForNull(message, nameof(message), "The message cannot be null");
            await RestClient.ProcessMessage(message);
        }

        public async Task SendMessageAsync(string message, bool isTTS = false)
        {
            Checks.CheckWebhookStatus(_status);
            Checks.CheckForArgument(string.IsNullOrWhiteSpace(message), nameof(message), "The message cannot be empty, null, or completely whitespace");
            WebhookMessage messageImpl = new WebhookMessage(message, isTTS);
            await RestClient.ProcessMessage(messageImpl);
        }

        public async Task<Exception> SendMessageAsyncSafely(IWebhookMessage message)
        {
            Checks.CheckWebhookStatus(_status);
            Checks.CheckForNull(message, nameof(message), "The message cannot be null");
            return await RestClient.ProcessMessage(message);
        }

        public async Task<Exception> SendMessageAsyncSafely(string message, bool isTTS = false)
        {
            Checks.CheckWebhookStatus(_status);
            Checks.CheckForArgument(string.IsNullOrWhiteSpace(message), nameof(message), "The message cannot be empty, null, or completely whitespace");
            WebhookMessage messageImpl = new WebhookMessage(message, isTTS);
            return await RestClient.ProcessMessage(messageImpl);
        }
    }
}
