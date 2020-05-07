using DSharp4Webhook.Logging;
using DSharp4Webhook.Rest.Entities;
using DSharp4Webhook.Rest.Manipulation;
using DSharp4Webhook.Rest.Mono.Util;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading;
using System.Threading.Tasks;

namespace DSharp4Webhook.Rest.Mono
{
    [ProviderPriority(0)]
    public sealed class MonoProvider : BaseRestProvider
    {
        /// <summary>
        ///     Caching policy for all requests.
        ///     Exists to minimize the cost of requests.
        /// </summary>
        private static readonly RequestCachePolicy _cachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

        /// <summary>
        ///     Sets the given provider is the default.
        /// </summary>
        public static void SetupAsDefault()
        {
            RestProviderLoader.SetProviderType(typeof(MonoProvider));
        }

        public MonoProvider(RestClient restClient, SemaphoreSlim locker) : base(restClient, locker) { }

        public override async Task<RestResponse[]> GET(string url, uint maxAttempts)
        {
            return await Raw("GET", url, GET_ALLOWED_STATUSES, maxAttempts);
        }

        public override async Task<RestResponse[]> POST(string url, string data, uint maxAttempts = 1)
        {
            return await Raw("POST", url, POST_ALLOWED_STATUSES, maxAttempts, data);
        }

        private async Task<RestResponse[]> Raw(string method, string url, HttpStatusCode[] allowedStatuses, uint maxAttempts = 1, string data = null)
        {
            Checks.CheckWebhookStatus(_restClient.Webhook.Status);
            Checks.CheckForArgument(string.IsNullOrEmpty(method), nameof(method));
            Checks.CheckForArgument(string.IsNullOrEmpty(url), nameof(url));
            Checks.CheckForNull(allowedStatuses);

            _locker.Wait();
            List<RestResponse> responses = new List<RestResponse>();

            uint currentAttimpts = 0;
            // Used to prevent calls if something went wrong
            bool forceStop = false;

            do
            {
                if (responses.Count != 0)
                    await _restClient.FollowRateLimit(responses.Last().RateLimit);


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
                    StreamUtil.Write(requestStream, data);

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
                        ProcessStatusCode(response.StatusCode, ref forceStop, allowedStatuses);
                    }
                }
                Log(new LogContext(LogSensitivity.VERBOSE, $"[A {currentAttimpts}] [SC {(int)responses.Last().StatusCode}] [RLR {restResponse.RateLimit.Reset:yyyy-MM-dd HH:mm:ss.fff zzz}] [RLMW {restResponse.RateLimit.MustWait}] Post request completed:{(restResponse.Content.Length != 0 ? string.Concat(Environment.NewLine, restResponse.Content) : " No content")}", _restClient.Webhook.Id));

                // first of all we check the forceStop so that we don't go any further if
            } while (!forceStop && (!allowedStatuses.Contains(responses.Last().StatusCode) && (maxAttempts > 0 ? ++currentAttimpts <= maxAttempts : true)));

            _locker.Release();
            return responses.ToArray();
        }
    }
}
