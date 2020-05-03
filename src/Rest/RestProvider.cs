﻿using DSharp4Webhook.Logging;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Net.Cache;

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
        public static async Task<RestResponse[]> POST(string url, string data, bool waitRatelimit = true, uint maxAttempts = 1, ulong dId = 0, RestClient client = null)
        {
            client?._locker.Wait();
            List<RestResponse> responses = new List<RestResponse>();
            uint currentAttimpts = 0;
            do
            {
                if (waitRatelimit && responses.Count != 0)
                    await Task.Delay(responses.Last().RateLimit.MustWait);


                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.CachePolicy = _cachePolicy;
                request.Method = "POST";
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
                        string content;
                        using (Stream responseStream = response.GetResponseStream())
                            content = StreamUtil.Read(responseStream);
                        RateLimitInfo rateLimitInfo = new RateLimitInfo(response.Headers.GetAsDictionary());
                        HttpStatusCode statusCode = response.StatusCode;
                        restResponse = new RestResponse(statusCode, rateLimitInfo, content, currentAttimpts);
                        responses.Add(restResponse);

                        response.Close();
                    }
                }

                LogProvider.Log(new LogContext(LogSensitivity.VERBOSE, $"[D {dId}] [A {currentAttimpts}] [SC {(int)responses.Last().StatusCode}] [RLR {restResponse.RateLimit.Reset:yyyy-MM-dd HH:mm:ss.fff zzz}] [RLMW {restResponse.RateLimit.MustWait}] Post request completed:{(restResponse.Content.Length != 0 ? string.Concat(Environment.NewLine, restResponse.Content) : " No content")}", client?.Parent));

            } while (responses.Last().StatusCode != HttpStatusCode.NoContent && (maxAttempts > 0 ? ++currentAttimpts <= maxAttempts : true));

            client?._locker.Release();
            return responses.ToArray();
        }
    }
}
