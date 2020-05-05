using DSharp4Webhook.Core;
using DSharp4Webhook.Logging;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

#if MONO_BUILD
using System.Net.Cache;
using System.IO;
#else
using System.Net.Http;
using System.Text;
#endif

namespace DSharp4Webhook.Rest
{
    /// <summary>
    ///     Responsible for REST interaction with the Discord API.
    /// </summary>
    public static class RestProvider
    {
#if MONO_BUILD
        /// <summary>
        ///     Caching policy for all requests.
        ///     Exists to minimize the cost of requests.
        /// </summary>
        private static readonly RequestCachePolicy _cachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
#endif
        /// <param name="maxAttempts">
        ///     The maximum number of attempts that can be made.
        ///     Set to 0 if you want infinite attempts.
        /// </param>
        /// <param name="client">
        ///     The client from which the request was sent may be null,
        ///     which will add incorrect request processing.
        /// </param>
#if MONO_BUILD
        public static async Task<RestResponse[]> POST(string url, string data, uint maxAttempts = 1, IWebhook client = null)
        {
            return await Raw("POST", url, maxAttempts, data, client);
        }
#else
        public static async Task<RestResponse[]> POST(HttpClient httpClient, string url, string data, uint maxAttempts = 1, IWebhook client = null)
        {
            Checks.CheckWebhookStatus(client?.Status ?? WebhookStatus.NOT_CHECKED);
            Checks.CheckForNull(httpClient, nameof(httpClient));
            Checks.CheckForArgument(string.IsNullOrEmpty(url), nameof(url));
            Checks.CheckForArgument(string.IsNullOrEmpty(data), nameof(data));

            client?.RestClient._locker.Wait();
            List<RestResponse> responses = new List<RestResponse>();
            uint currentAttimpts = 0;
            // Used to prevent calls if something went wrong
            bool forceStop = false;

            do
            {
                if (responses.Count != 0)
                    await FollowRateLimit(responses.Last().RateLimit, client);

                // Need 'multipart/form-data' to send files
                HttpResponseMessage response = await httpClient.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));
                RateLimitInfo rateLimitInfo = new RateLimitInfo(response.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault()));
                RestResponse restResponse = new RestResponse(response.StatusCode, rateLimitInfo, await response.Content.ReadAsStringAsync(), currentAttimpts);

                // Processing the necessary status codes
                ProcessStatusCode(response.StatusCode, ref forceStop, client);
                client?.Provider?.Log(new LogContext(LogSensitivity.VERBOSE, $"[A {currentAttimpts}] [SC {(int)responses.Last().StatusCode}] [RLR {restResponse.RateLimit.Reset:yyyy-MM-dd HH:mm:ss.fff zzz}] [RLMW {restResponse.RateLimit.MustWait}] Post request completed:{(restResponse.Content.Length != 0 ? string.Concat(Environment.NewLine, restResponse.Content) : " No content")}", client?.Id));

            } while (!forceStop && (responses.Last().StatusCode != HttpStatusCode.NoContent && (maxAttempts > 0 ? ++currentAttimpts <= maxAttempts : true)));

            client?.RestClient._locker.Release();
            return responses.ToArray();
        }
#endif

#if MONO_BUILD
        /// <remarks>
        ///     Wrapper for all requests.
        /// </remarks>
        private static async Task<RestResponse[]> Raw(string method, string url, uint maxAttempts = 1, string content = null, IWebhook client = null)
        {
            Checks.CheckWebhookStatus(client?.Status ?? WebhookStatus.NOT_CHECKED);
            Checks.CheckForArgument(string.IsNullOrEmpty(method), nameof(method));
            Checks.CheckForArgument(string.IsNullOrEmpty(url), nameof(url));
            client?.RestClient._locker.Wait();
            List<RestResponse> responses = new List<RestResponse>();

            uint currentAttimpts = 0;
            // Used to prevent calls if something went wrong
            bool forceStop = false;

            do
            {
                if (responses.Count != 0)
                    await FollowRateLimit(responses.Last().RateLimit, client);


                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.CachePolicy = _cachePolicy;
                request.Method = method;
                // Identify themselves
                request.UserAgent = "DSharp4Webhook";
                // Need 'multipart/form-data' to send files
                // todo: sending files
                request.ContentType = "application/json";
                // Uses it for accurate measurement RateLimit
                request.Headers.Set("X-RateLimit-Precision", "millisecond");
                // Disabling keep-alive, this is a one-time connection
                request.KeepAlive = false;
                // I noticed a memory leak on a stress test
                // It wat because System.PinnableBufferCache not cleared
                // If we use 'request.AllowWriteStreamBuffering = false', it just stops working and throwing an WebException

                RestResponse restResponse;
                using (Stream requestStream = request.GetRequestStream())
                {
                    StreamUtil.Write(requestStream, content);

                    using (HttpWebResponse response = request.GetResponseNoException())
                    {
                        string responseContent;
                        using (Stream responseStream = response.GetResponseStream())
                            responseContent = StreamUtil.Read(responseStream);
                        RateLimitInfo rateLimitInfo = new RateLimitInfo(response.Headers.GetAsDictionary());
                        HttpStatusCode statusCode = response.StatusCode;
                        restResponse = new RestResponse(statusCode, rateLimitInfo, responseContent, currentAttimpts);
                        responses.Add(restResponse);

                        response.Close();

                        // Processing the necessary status codes
                        ProcessStatusCode(response.StatusCode, ref forceStop, client);
                    }
                }
                client?.Provider?.Log(new LogContext(LogSensitivity.VERBOSE, $"[A {currentAttimpts}] [SC {(int)responses.Last().StatusCode}] [RLR {restResponse.RateLimit.Reset:yyyy-MM-dd HH:mm:ss.fff zzz}] [RLMW {restResponse.RateLimit.MustWait}] Post request completed:{(restResponse.Content.Length != 0 ? string.Concat(Environment.NewLine, restResponse.Content) : " No content")}", client?.Id));
        // first of all we check the forceStop so that we don't go any further if
    } while (!forceStop && (responses.Last().StatusCode != HttpStatusCode.NoContent && (maxAttempts > 0 ? ++currentAttimpts <= maxAttempts : true)));

            client?.RestClient._locker.Release();
            return responses.ToArray();
        }
#endif

        /// <summary>
        ///     Waits for the specified rate limit to expire.
        /// </summary>
        public static async Task FollowRateLimit(RateLimitInfo rateLimit, IWebhook client = null)
        {
            TimeSpan mustWait = rateLimit.MustWait;
            if (mustWait != TimeSpan.Zero)
            {
                client?.Provider?.Log(new LogContext(LogSensitivity.INFO, $"Saving for {mustWait.TotalMilliseconds}ms", client?.Id));
                await Task.Delay(mustWait).ConfigureAwait(false);
            }
        }

        /// <remarks>
        ///     Wrapper for processing returned status codes.
        /// </remarks>
        private static void ProcessStatusCode(HttpStatusCode statusCode, ref bool forceStop, IWebhook client)
        {
            // Processing the necessary status codes
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    if (client != null)
                        client.Status = WebhookStatus.NOT_EXISTING;
                    client?.Provider?.Log(new LogContext(LogSensitivity.ERROR, "A REST request of the POST type returned 404, the webhack does not exist, and we are deleting it...", client?.Id));
                    forceStop = true;
                    client?.Dispose();
                    break;
                case HttpStatusCode.BadRequest:
                    client?.Provider?.Log(new LogContext(LogSensitivity.ERROR, "A REST request of the POST type returnet 400, something went wrong...", client?.Id));
                    forceStop = true;
                    break;
            }
        }
    }
}
