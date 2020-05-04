using DSharp4Webhook.Logging;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading.Tasks;

namespace DSharp4Webhook.Rest
{
    /// <summary>
    ///     Responsible for REST interaction with the Discord API.
    /// </summary>
    public static class RestProvider
    {
        private static readonly RequestCachePolicy _cachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

        /// <param name="maxAttempts">
        ///     The maximum number of attempts that can be made.
        ///     Set to 0 if you want infinite attempts.
        /// </param>
        /// <param name="client">
        ///     Responsible solely for logging in the context of a single webhook.
        /// </param>
        public static async Task<RestResponse[]> POST(string url, string data, uint maxAttempts = 1, RestClient client = null)
        {
            return await Raw("POST", url, maxAttempts, data, client);
        }

        /// <remarks>
        ///     Wrapper for all requests.
        /// </remarks>
        private static async Task<RestResponse[]> Raw(string method, string url, uint maxAttempts = 1, string content = null, RestClient client = null)
        {
            client?._locker.Wait();
            List<RestResponse> responses = new List<RestResponse>();
            uint currentAttimpts = 0;
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
                    }
                }

                LogProvider.Log(new LogContext(LogSensitivity.VERBOSE, $"[A {currentAttimpts}] [SC {(int)responses.Last().StatusCode}] [RLR {restResponse.RateLimit.Reset:yyyy-MM-dd HH:mm:ss.fff zzz}] [RLMW {restResponse.RateLimit.MustWait}] Post request completed:{(restResponse.Content.Length != 0 ? string.Concat(Environment.NewLine, restResponse.Content) : " No content")}", client?.Source));

            } while (responses.Last().StatusCode != HttpStatusCode.NoContent && (maxAttempts > 0 ? ++currentAttimpts <= maxAttempts : true));

            client?._locker.Release();
            return responses.ToArray();
        }

        public static async Task FollowRateLimit(RateLimitInfo rateLimit, RestClient client = null)
        {
            TimeSpan mustWait = rateLimit.MustWait;
            if (mustWait != TimeSpan.Zero)
            {
                LogProvider.Log(new LogContext(LogSensitivity.INFO, $"Saving for {mustWait.TotalMilliseconds}ms", client?.Source));
                await Task.Delay(mustWait).ConfigureAwait(false);
            }
        }
    }
}
