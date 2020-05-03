using DSharp4Webhook.Core;
using DSharp4Webhook.Entities;
using DSharp4Webhook.Logging;
using DSharp4Webhook.Util;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DSharp4Webhook.Rest
{
    /// <summary>
    ///     Rest worker for each webhook.
    /// </summary>
    public class RestClient : IDisposable
    {
        public IWebhook Parent { get; }

        // Tracks dispose.
        private bool _isntDisposed;
        private RateLimitInfo? _rateLimitInfo;
        internal readonly SemaphoreSlim _locker;
        private Task _worker;

        public RestClient(IWebhook webhook)
        {
            Parent = webhook;
            _isntDisposed = true;
            _rateLimitInfo = null;
            _locker = new SemaphoreSlim(1, 1);

            Start();
        }

        /// <summary>
        ///     Initializes our task if it has not yet been initialized or failed.
        /// </summary>
        public bool Start()
        {
            if (_isntDisposed && (_worker == null || _worker.IsCompleted || _worker.IsFaulted))
            {
                LogProvider.Log(new LogContext(LogSensitivity.VERBOSE, "Initialize worker", Parent));
                _worker = Task.Run(Do);
                return true;
            }
            return false;
        }

        private async void Do()
        {
            while (_isntDisposed)
            {
                if (Parent.MessageQueue.TryDequeue(out IWebhookMessage message))
                {
                    LogProvider.Log(new LogContext(LogSensitivity.VERBOSE, $"[D {message.DeliveryId}] Processing message with content: {(message.Content.Length < 60 ? message.Content : string.Concat(message.Content.Substring(0, 30), "..."))}", Parent));
                    Exception ex;
                    if ((ex = await ProcessMessage(message, true)) != null)
                    {
                        // If we are'nt able to filter
                        throw ex;
                    }
                    LogProvider.Log(new LogContext(LogSensitivity.INFO, $"[D {message.DeliveryId}] The message is sent", Parent));
                }
                else
                {
                    // Passing the execution context to another
                    await Task.Yield();
                    // If there are still no messages, we're waiting
                    if (Parent.MessageQueue.Count == 0)
                        await Task.Delay(150);
                }
            }
        }

        public async Task SendMessage(IWebhookMessage message, bool waitForRatelimit = true, uint maxAttempts = 1)
        {
            RateLimitInfo? ratelimit = GetRateLimit();
            if (waitForRatelimit && ratelimit.HasValue)
            {
                TimeSpan mustWait = ratelimit.Value.MustWait;
                if (mustWait != TimeSpan.Zero)
                {
                    LogProvider.Log(new LogContext(LogSensitivity.INFO, $"[D {message.DeliveryId}] Saving for {mustWait.TotalMilliseconds}ms", Parent));
                    await Task.Delay(ratelimit.Value.MustWait).ConfigureAwait(false);
                }
            }

            RestResponse[] responses = await RestProvider.POST(Parent.GetWebhookUrl(), JsonConvert.SerializeObject(message), waitForRatelimit, maxAttempts, message.DeliveryId, this);
            RestResponse lastResponse = responses[responses.Length - 1];
            SetRateLimit(lastResponse.RateLimit);
            LogProvider.Log(new LogContext(LogSensitivity.VERBOSE, $"[RC {responses.Length}] [D {message.DeliveryId}] [A {lastResponse.Attempts}] Successful POST request", Parent));
        }

        public async Task<Exception> ProcessMessage(IWebhookMessage message, bool waitForRatelimit = true, uint maxAttempts = 1)
        {
            try
            {
                await SendMessage(message, waitForRatelimit, maxAttempts);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case WebException webException:
                        LogProvider.Log(new LogContext(LogSensitivity.WARN, $"[D {message.DeliveryId}] WebException {(int)webException.Status}-{((int?)(webException.Response as HttpWebResponse)?.StatusCode) ?? -1}: {webException.Message}", Parent));
                        LogProvider.Log(new LogContext(LogSensitivity.DEBUG, $"[D {message.DeliveryId}] StackTrace:\n{webException.StackTrace}", Parent));
                        return null;
                    default:
                        LogProvider.Log(new LogContext(LogSensitivity.ERROR, $"[D {message.DeliveryId}] Unhandled exception: {ex.Source} {ex.Message}", Parent));
                        LogProvider.Log(new LogContext(LogSensitivity.DEBUG, $"[D {message.DeliveryId}] StackTrace:\n{ex.StackTrace}", Parent));
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
