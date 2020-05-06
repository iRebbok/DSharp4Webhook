using DSharp4Webhook.Core;
using DSharp4Webhook.Logging;
using DSharp4Webhook.Rest.Entities;
using DSharp4Webhook.Rest.Manipulation;
using DSharp4Webhook.Util;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DSharp4Webhook.Rest
{
    /// <summary>
    ///     Rest worker for each webhook.
    /// </summary>
    public class RestClient : IDisposable
    {
        public IWebhook Webhook { get => _webhook; }

        // Tracks dispose.
        private bool _isntDisposed;
        private RateLimitInfo? _rateLimitInfo;
        private readonly SemaphoreSlim _locker;
        private readonly IWebhook _webhook;
        private readonly BaseRestProvider _provider;
        private Task _worker;

        public RestClient(IWebhook webhook)
        {
            _isntDisposed = true;
            _rateLimitInfo = null;
            _locker = new SemaphoreSlim(1, 1);
            _webhook = webhook;

            _provider = RestProviderLoader.CreateProvider(this, _locker);
            // Immediately launch the worker
            Start();
        }

        /// <summary>
        ///     Initializes our task if it has not yet been initialized or failed.
        /// </summary>
        public bool Start()
        {
            if (_isntDisposed && (_worker == null || _worker.IsCompleted || _worker.IsFaulted))
            {
                _webhook.Provider?.Log(new LogContext(LogSensitivity.VERBOSE, "Initialize worker", _webhook.Id));
                _worker = Task.Run(Do);
                return true;
            }
            return false;
        }

        private async void Do()
        {
            while (_isntDisposed)
            {
                // Destroyed if the status is unsuitable for iteration
                if (_webhook.Status == WebhookStatus.NOT_EXISTING)
                    break;

                if (_webhook.MessageQueue.TryDequeue(out IWebhookMessage message))
                {
                    _webhook.Provider?.Log(new LogContext(LogSensitivity.VERBOSE, $"Processing message with content: {(message.Content.Length < 60 ? message.Content : string.Concat(message.Content.Substring(0, 30), "..."))}", _webhook.Id));

                    Exception ex;
                    if ((ex = await ProcessMessage(message)) != null)
                    {

                        if (!(ex is WebException))
                            throw ex;
                    }
                    _webhook.Provider?.Log(new LogContext(LogSensitivity.INFO, "The message is sent", _webhook.Id, ex));
                }
                else
                {
                    // Passing the execution context to another
                    await Task.Yield();
                    // If there are still no messages, we're waiting
                    if (_webhook.MessageQueue.Count == 0)
                        await Task.Delay(150);
                }
            }
        }

        /// <summary>
        ///     Asynchronous sends the specified message.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     If the message is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If the webhook is unusable.
        /// </exception>
        public async Task SendMessage(IWebhookMessage message, uint maxAttempts = 1)
        {
            Checks.CheckForNull(message, nameof(message), "The message cannot be null");
            Checks.CheckWebhookStatus(_webhook.Status);
            RateLimitInfo? ratelimit = GetRateLimit();
            if (ratelimit.HasValue)
                await FollowRateLimit(ratelimit.Value);

            message = (IWebhookMessage)Merger.Merge(_webhook.WebhookInfo, message);
            RestResponse[] responses = await _provider.POST(_webhook.GetWebhookUrl(), JsonConvert.SerializeObject(message), maxAttempts);
            RestResponse lastResponse = responses[responses.Length - 1];
            SetRateLimit(lastResponse.RateLimit);
            _webhook.Provider?.Log(new LogContext(LogSensitivity.VERBOSE, $"[RC {responses.Length}] [A {lastResponse.Attempts}] Successful POST request", _webhook.Id));
        }

        /// <summary>
        ///     Processes the message properly.
        /// </summary>
        /// <returns>
        ///     Exception if it was thrown.
        /// </returns>
        public async Task<Exception> ProcessMessage(IWebhookMessage message, uint maxAttempts = 1)
        {
            try
            {
                await SendMessage(message, maxAttempts);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case WebException webException:
                        _webhook.Provider?.Log(new LogContext(LogSensitivity.WARN, $"WebException {(int)webException.Status}-{((int?)(webException.Response as HttpWebResponse)?.StatusCode) ?? -1}: {webException.Message}", _webhook.Id, ex));
                        _webhook.Provider?.Log(new LogContext(LogSensitivity.DEBUG, $"StackTrace:\n{webException.StackTrace}", _webhook.Id, ex));
                        return ex;
                    default:
                        _webhook.Provider?.Log(new LogContext(LogSensitivity.ERROR, $"Unhandled exception: {ex.Source} {ex.Message}", _webhook.Id, ex));
                        _webhook.Provider?.Log(new LogContext(LogSensitivity.DEBUG, $"StackTrace:\n{ex.StackTrace}", _webhook.Id, ex));
                        break;
                }

                return ex;
            }
            return null;
        }

        public void SetRateLimit(RateLimitInfo rateLimit)
        {
            _locker.Wait();
            _rateLimitInfo = rateLimit;
            _locker.Release();
        }

        public RateLimitInfo? GetRateLimit()
        {
            RateLimitInfo? result;
            _locker.Wait();
            result = _rateLimitInfo;
            _locker.Release();
            return result;
        }

        /// <summary>
        ///     Waits for the specified rate limit to expire.
        /// </summary>
        public async Task FollowRateLimit(RateLimitInfo rateLimit)
        {
            TimeSpan mustWait = rateLimit.MustWait;
            if (mustWait != TimeSpan.Zero)
            {
                _webhook.Provider?.Log(new LogContext(LogSensitivity.INFO, $"Saving for {mustWait.TotalMilliseconds}ms", _webhook.Id));
                await Task.Delay(mustWait).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _isntDisposed = false;
        }
    }
}