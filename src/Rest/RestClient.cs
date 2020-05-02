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
using System.Threading.Tasks;

namespace DSharp4Webhook.Rest
{
    /// <summary>
    ///     Rest worker for each webhook.
    /// </summary>
    public class RestClient : IDisposable
    {
        private readonly IWebhook _webhook;

        // Tracks dispose.
        private bool _isntDisposed = true;

        /// <summary>
        ///     Determines whether the client is in the RateLimit.
        /// </summary>
        public bool IsRatelimited => MustWait != 0;

        public uint MustWait
        {
            get
            {
                long unixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (ratelimitReset < unixSeconds)
                    return 0;
                int value = (int)(ratelimitReset - unixSeconds);
                return value > 0 ? (uint)value : 0;
            }
        }

        // It stores the time in unix format seconds when the reset occurs
        private long ratelimitReset;
        private Task _worker;

        public RestClient(IWebhook webhook)
        {
            _webhook = webhook;
            Start();
        }

        /// <summary>
        ///     Initializes our task if it has not yet been initialized or failed.
        /// </summary>
        public bool Start()
        {
            if (_worker == null || _worker.IsCompleted || _worker.IsFaulted)
            {
                LogProvider.Log(_webhook, new LogContext(LogSensitivity.VERBOSE, $"Initialize worker", true));
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
                    Exception ex;
                    if ((ex = await ProcessMessage(message.DeliveryId, message, true)) != null)
                    {
                        // If you are'nt able to filter
                        throw ex;
                    }
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

        public async Task SendMessage(ulong dId, IWebhookMessage message, bool waitForRatelimit = true)
        {
            uint mustWait;
            if (waitForRatelimit && (mustWait = MustWait) != 0)
            {
                // Expect more because the server may send that we are in ratelimit and we still have requests
                int waitFor = (int)mustWait * 1000 + 2400;
                LogProvider.Log(_webhook, new LogContext(LogSensitivity.VERBOSE, $"Waiting for {waitFor}ms"));
                await Task.Delay(waitFor);
            }

            // Combining constant data
            message = (IWebhookMessage)Merger.Merge(_webhook.WebhookData, message);
            HttpWebRequest request = WebRequest.CreateHttp(_webhook.GetWebhookUrl());
            // Need 'multipart/form-data' to send files
            request.ContentType = "application/json";
            // Uses it for accurate measurement
            request.Headers.Set("X-RateLimit-Precision", "millisecond");
            // Identify themselves
            request.UserAgent = "DSharp4Webhook";
            // Disabling keep-alive, this is a one-time connection
            request.KeepAlive = false;
            request.Method = "POST";
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            ProcessResponce(dId, response);
            response.Close();
            response.Dispose();
        }

        // todo: completely rewrite the rate limit provider
        private void ProcessResponce(ulong dId, HttpWebResponse response)
        {
            // First of all we process the RateLimit
            if (byte.TryParse(response.Headers.Get("x-ratelimit-remaining"), NumberStyles.Any, CultureInfo.InvariantCulture, out byte requestsCanBeMade) && requestsCanBeMade < 2 &&
                // I don't know why, but it simply didn't parse without additional delimiting arguments
                decimal.TryParse(response.Headers.Get("x-ratelimit-reset"), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal reset))
            {
                // Decimal due to using milliseconds instead of seconds
                ratelimitReset = Convert.ToInt64(Math.Ceiling(reset));
                LogProvider.Log(_webhook, new LogContext(LogSensitivity.VERBOSE, $"We're retelimited: {ratelimitReset}", true));
            }

            // Basically a 200 code trigger
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                LogProvider.Log(_webhook, new LogContext(LogSensitivity.WARN, $"[D {dId}] The status return code is'nt 204: {response.StatusCode}", true));
                string content;
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                    content = reader.ReadToEnd();
                LogProvider.Log(_webhook, new LogContext(LogSensitivity.DEBUG, $"[D {dId}] The returned content:\n{content}"));
            }
        }

        public async Task<Exception> ProcessMessage(ulong dId, IWebhookMessage message, bool waitForRatelimit = true)
        {
            try
            {
                await SendMessage(dId, message, true);
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case WebException webException:
                        LogProvider.Log(_webhook, new LogContext(LogSensitivity.WARN, $"[D {message.DeliveryId}] WebException {webException.Status}: {webException.Message}", true));
                        LogProvider.Log(_webhook, new LogContext(LogSensitivity.DEBUG, $"[D {message.DeliveryId}] StackTrace:\n{webException.StackTrace}"));
                        return null;
                    default:
                        LogProvider.Log(_webhook, new LogContext(LogSensitivity.ERROR, $"[D {message.DeliveryId}] Unhandled exception: {ex.Source} {ex.Message}"));
                        LogProvider.Log(_webhook, new LogContext(LogSensitivity.DEBUG, $"[D {message.DeliveryId}] StackTrace:\n{ex.StackTrace}"));
                        break;
                }

                return ex;
            }
            return null;
        }

        public void Dispose()
        {
            _isntDisposed = false;
        }
    }
}
