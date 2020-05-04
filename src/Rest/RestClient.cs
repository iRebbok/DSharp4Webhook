using DSharp4Webhook.Core;
using DSharp4Webhook.Logging;
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
        public IWebhook Source { get => _webhook; }

        // Tracks dispose.
        private bool _isntDisposed;
        private RateLimitInfo? _rateLimitInfo;
        internal readonly SemaphoreSlim _locker;
        private readonly IWebhook _webhook;
        private Task _worker;

        public RestClient(IWebhook webhook)
        {
            _isntDisposed = true;
            _rateLimitInfo = null;
            _locker = new SemaphoreSlim(1, 1);
            _webhook = webhook;

            Start();
        }

        /// <summary>
        ///     Initializes our task if it has not yet been initialized or failed.
        /// </summary>
        public bool Start()
        {
            if (_isntDisposed && (_worker == null || _worker.IsCompleted || _worker.IsFaulted))
            {
                LogProvider.Log(new LogContext(LogSensitivity.VERBOSE, "Initialize worker", _webhook));
                _worker = Task.Run(Do);
                return true;
            }
            return false;
        }

        private async void Do()
        {
            while (_isntDisposed)
            {
                if (_webhook.MessageQueue.TryDequeue(out IWebhookMessage message))
                {
                    LogProvider.Log(new LogContext(LogSensitivity.VERBOSE, $"Processing message with content: {(message.Content.Length < 60 ? message.Content : string.Concat(message.Content.Substring(0, 30), "..."))}", _webhook));
                    Exception ex;
                    if ((ex = await ProcessMessage(message)) != null)
                    {
                        // If we are'nt able to filter
                        throw ex;
                    }
                    LogProvider.Log(new LogContext(LogSensitivity.INFO, $"The message is sent", _webhook));
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

        public async Task SendMessage(IWebhookMessage message, uint maxAttempts = 1)
        {
            RateLimitInfo? ratelimit = GetRateLimit();
            if (ratelimit.HasValue)
                await RestProvider.FollowRateLimit(ratelimit.Value, this);

            message = (IWebhookMessage)Merger.Merge(_webhook.WebhookInfo, message);
            RestResponse[] responses = await RestProvider.POST(_webhook.GetWebhookUrl(), JsonConvert.SerializeObject(message), maxAttempts, this);
            RestResponse lastResponse = responses[responses.Length - 1];
            SetRateLimit(lastResponse.RateLimit);
            LogProvider.Log(new LogContext(LogSensitivity.VERBOSE, $"[RC {responses.Length}] [A {lastResponse.Attempts}] Successful POST request", _webhook));
        }

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
                        LogProvider.Log(new LogContext(LogSensitivity.WARN, $"WebException {(int)webException.Status}-{((int?)(webException.Response as HttpWebResponse)?.StatusCode) ?? -1}: {webException.Message}", _webhook));
                        LogProvider.Log(new LogContext(LogSensitivity.DEBUG, $"StackTrace:\n{webException.StackTrace}", _webhook));
                        return null;
                    default:
                        LogProvider.Log(new LogContext(LogSensitivity.ERROR, $"Unhandled exception: {ex.Source} {ex.Message}", _webhook));
                        LogProvider.Log(new LogContext(LogSensitivity.DEBUG, $"StackTrace:\n{ex.StackTrace}", _webhook));
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

        public void Dispose()
        {
            _isntDisposed = false;
        }
    }
}
