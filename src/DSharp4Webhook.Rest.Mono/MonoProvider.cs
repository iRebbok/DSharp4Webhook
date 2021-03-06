using DSharp4Webhook.Core;
using DSharp4Webhook.Logging;
using DSharp4Webhook.Rest.Manipulation;
using DSharp4Webhook.Rest.Mono.Util;
using DSharp4Webhook.Serialization;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Threading.Tasks;

namespace DSharp4Webhook.Rest.Mono
{
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

        public MonoProvider(IWebhook webhook) : base(webhook) { }

        public override async Task<RestResponse[]> GET(string url, RestSettings restSettings)
        {
            return await Raw("GET", url, GET_ALLOWED_STATUSES, restSettings).ConfigureAwait(false);
        }

        public override async Task<RestResponse[]> POST(string url, SerializationContext data, RestSettings restSettings)
        {
            return await Raw("POST", url, POST_ALLOWED_STATUSES, restSettings, data).ConfigureAwait(false);
        }

        public override async Task<RestResponse[]> DELETE(string url, RestSettings restSettings)
        {
            return await Raw("DELETE", url, DELETE_ALLOWED_STATUSES, restSettings).ConfigureAwait(false);
        }

        public override async Task<RestResponse[]> PATCH(string url, SerializationContext data, RestSettings restSettings)
        {
            Contract.AssertRequiredSerizationType(data, SerializationType.APPLICATION_JSON);
            return await Raw("PATH", url, PATCH_ALLOWED_STATUSES, restSettings, data).ConfigureAwait(false);
        }

        private async Task<RestResponse[]> Raw(string method, string url, IReadOnlyCollection<HttpStatusCode> allowedStatuses, RestSettings restSettings, SerializationContext? data = null)
        {
            Contract.EnsureWebhookIsNotBroken(_webhook.Status);
            Contract.AssertArgumentNotTrue(string.IsNullOrEmpty(method), nameof(method));
            Contract.AssertArgumentNotTrue(string.IsNullOrEmpty(url), nameof(url));
            Contract.AssertNotNull(allowedStatuses, nameof(allowedStatuses));
            Contract.CheckForNull(restSettings, nameof(restSettings));

            List<RestResponse> responses = new List<RestResponse>();

            uint currentAttimpts = 0;
            // Used to prevent calls if something went wrong
            bool forceStop = false;

            do
            {
                if (responses.Count != 0)
                    await _webhook.ActionManager.FollowRateLimit(responses.Last().RateLimit).ConfigureAwait(false);

                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.CachePolicy = _cachePolicy;
                request.Method = method;
                // Calling 'GetRequestStream()' after setting the request type
                using var requestStream = request.GetRequestStream();
                PrepareRequest(request, requestStream, data);
                // Identify themselves
                request.UserAgent = $"DSharp4Webhook ({WebhookProvider.LibraryUrl}, {WebhookProvider.LibraryVersion})";
                // The content type is assigned in 'PrepareRequest'
                // Uses it for accurate measurement RateLimit
                request.Headers.Set("X-RateLimit-Precision", "millisecond");
                // Disabling keep-alive, this is a one-time connection
                request.KeepAlive = false;
                // I noticed a memory leak on a stress test
                // It wat because System.PinnableBufferCache not cleared
                // If we use 'request.AllowWriteStreamBuffering = false', it just stops working and throwing an WebException

                RestResponse restResponse;
                using (HttpWebResponse response = request.GetResponseNoException())
                {
                    RateLimitInfo rateLimitInfo = new RateLimitInfo(response.Headers.GetAsDictionary());
                    restResponse = new RestResponse(response, rateLimitInfo, currentAttimpts);
                    responses.Add(restResponse);

                    // Processing the necessary status codes
                    ProcessStatusCode(response.StatusCode, ref forceStop, allowedStatuses);
                }
                Log(new LogContext(LogSensitivity.VERBOSE, $"[A {currentAttimpts}] [SC {(int)responses.Last().StatusCode}] [RLR {restResponse.RateLimit.Reset:yyyy-MM-dd HH:mm:ss.fff zzz}] [RLMW {restResponse.RateLimit.MustWait}] Post request completed:{(restResponse.Content?.Length != 0 ? string.Concat(Environment.NewLine, restResponse.Content ?? string.Empty) : " No content")}", _webhook.Id));

                // first of all we check the forceStop so that we don't go any further if
#pragma warning disable IDE0075 // Simplify conditional expression
            } while (!forceStop && (!allowedStatuses.Contains(responses.Last().StatusCode) && (restSettings.Attempts > 0 ? ++currentAttimpts <= restSettings.Attempts : true)));
#pragma warning restore IDE0075 // Simplify conditional expression

            return responses.ToArray();
        }

        /// <summary>
        ///     Prepares the request.
        /// </summary>
        private static void PrepareRequest(HttpWebRequest request, Stream requestStream, SerializationContext? data = null)
        {
            if (data is null) return;
            SerializationContext context = data.Value;

            switch (context.Type)
            {
                case SerializationType.APPLICATION_JSON:
                {
                    request.ContentType = SerializeTypeConverter.Convert(SerializationType.APPLICATION_JSON);
                    // Writing a serialized context, no more
                    requestStream.Write(context.Content.ToArray(), 0, context.Content!.Count);
                    break;
                }
                case SerializationType.MULTIPART_FORM_DATA:
                {
                    MultipartHelper.PrepareMultipartFormDataRequest(request, requestStream, context);
                    break;
                }
            }
        }

        public override void Dispose()
        {
            // nothing to dispose
            GC.SuppressFinalize(this);
        }
    }
}
