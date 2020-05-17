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
using System.Text;
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

        public MonoProvider(IWebhook webhook) : base(webhook) { }

        public override async Task<RestResponse[]> GET(string url, uint maxAttempts)
        {
            return await Raw("GET", url, GET_ALLOWED_STATUSES, maxAttempts);
        }

        public override async Task<RestResponse[]> POST(string url, SerializeContext data, uint maxAttempts = 1)
        {
            return await Raw("POST", url, POST_ALLOWED_STATUSES, maxAttempts, data);
        }

        public override async Task<RestResponse[]> DELETE(string url, uint maxAttempts = 1)
        {
            return await Raw("DELETE", url, DELETE_ALLOWED_STATUSES, maxAttempts);
        }

        public override async Task<RestResponse[]> PATCH(string url, SerializeContext data, uint maxAttempts = 1)
        {
            Checks.CheckForArgument(data.Type != SerializeType.APPLICATION_JSON, nameof(data), "API do not support sending PATH as 'multipart/from-data'");
            return await Raw("PATH", url, PATCH_ALLOWED_STATUSES, maxAttempts, data);
        }

        private async Task<RestResponse[]> Raw(string method, string url, HttpStatusCode[] allowedStatuses, uint maxAttempts = 1, SerializeContext? data = null)
        {
            Checks.CheckWebhookStatus(_webhook.Status);
            Checks.CheckForArgument(string.IsNullOrEmpty(method), nameof(method));
            Checks.CheckForArgument(string.IsNullOrEmpty(url), nameof(url));
            Checks.CheckForNull(allowedStatuses);

            List<RestResponse> responses = new List<RestResponse>();

            uint currentAttimpts = 0;
            // Used to prevent calls if something went wrong
            bool forceStop = false;

            do
            {
                if (responses.Count != 0)
                    await _webhook.ActionManager.FollowRateLimit(responses.Last().RateLimit);


                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.CachePolicy = _cachePolicy;
                request.Method = method;
                // Calling 'GetRequestStream()' after setting the request type
                PrepareRequest(request, data);
                // Identify themselves
                request.UserAgent = "DSharp4Webhook";
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

                    response.Close();

                    // Processing the necessary status codes
                    ProcessStatusCode(response.StatusCode, ref forceStop, allowedStatuses);
                }
                Log(new LogContext(LogSensitivity.VERBOSE, $"[A {currentAttimpts}] [SC {(int)responses.Last().StatusCode}] [RLR {restResponse.RateLimit.Reset:yyyy-MM-dd HH:mm:ss.fff zzz}] [RLMW {restResponse.RateLimit.MustWait}] Post request completed:{(restResponse.Content.Length != 0 ? string.Concat(Environment.NewLine, restResponse.Content) : " No content")}", _webhook.Id));

                // first of all we check the forceStop so that we don't go any further if
            } while (!forceStop && (!allowedStatuses.Contains(responses.Last().StatusCode) && (maxAttempts > 0 ? ++currentAttimpts <= maxAttempts : true)));

            return responses.ToArray();
        }

        private const string fileHeaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n\r\n";
        private const string headerTemplate = "Content-Disposition: form-data; name=\"payload_json\"\r\n\r\n";

        /// <summary>
        ///     Prepares the request.
        /// </summary>
        private void PrepareRequest(HttpWebRequest request, SerializeContext? data = null)
        {
            if (data == null) return;
            SerializeContext context = data.Value;

            switch (context.Type)
            {
                case SerializeType.APPLICATION_JSON:
                    {
                        request.ContentType = SerializeTypeConverter.Convert(SerializeType.APPLICATION_JSON);
                        // Writing a serialized context, no more
                        using (var resuestStream = request.GetRequestStream())
                            resuestStream.Write(context.Content, 0, context.Content.Length);
                        break;
                    }
                case SerializeType.MULTIPART_FROM_DATA:
                    {
                        string boundary = $"--------------------------{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                        request.ContentType = $"{SerializeTypeConverter.Convert(SerializeType.MULTIPART_FROM_DATA)} ;boundary={boundary}";
                        // Inserted in the spaces between data
                        byte[] boundarySerialized = Encoding.ASCII.GetBytes($"\r\n--{boundary}\r\n");
                        // Inserted at the end of the data
                        byte[] boundarySerializedClose = Encoding.ASCII.GetBytes($"\r\n--{boundary}--\r\n");

                        using (var resuestStream = request.GetRequestStream())
                        {
                            if (context.Files != null && context.Files.Keys.Count != 0)
                            {
                                resuestStream.Write(boundarySerialized, 0, boundarySerialized.Length);

                                int index = 0;
                                foreach (var filePair in context.Files)
                                {
                                    index++;

                                    StreamUtil.Write(resuestStream, string.Format(fileHeaderTemplate, $"file{index}", filePair.Key), Encoding.ASCII);
                                    resuestStream.Write(filePair.Value, 0, filePair.Value.Length);

                                    // If this is not the last file
                                    // If you put it before, you risk breaking the body of the request
                                    if (index != context.Files.Keys.Count - 1)
                                        resuestStream.Write(boundarySerialized, 0, boundarySerialized.Length);

                                }
                            }

                            if (context.Content != null)
                            {
                                resuestStream.Write(boundarySerialized, 0, boundarySerialized.Length);
                                StreamUtil.Write(resuestStream, headerTemplate, Encoding.ASCII);
                                resuestStream.Write(context.Content, 0, context.Content.Length);
                            }

                            resuestStream.Write(boundarySerializedClose, 0, boundarySerializedClose.Length);
                        }
                        break;
                    }
            }
        }
    }
}
