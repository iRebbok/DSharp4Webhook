using DSharp4Webhook.Logging;
using DSharp4Webhook.Rest.Entities;
using DSharp4Webhook.Rest.Manipulation;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DSharp4Webhook.Rest.Default
{
    // Higher priority due to using this type of provider due to some issues about the Mono type
    [ProviderPriority(1)]
    public sealed class DefaultProvider : BaseRestProvider
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        ///     Sets the given provider is the default.
        /// </summary>
        public static void SetupAsDefault()
        {
            RestProviderLoader.SetProviderType(typeof(DefaultProvider));
        }

        public DefaultProvider(RestClient restClient, SemaphoreSlim locker) : base(restClient, locker)
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.UserAgent.Clear();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("DSharp4Webhook");
            _httpClient.DefaultRequestHeaders.Add("X-RateLimit-Precision", "millisecond");
        }

        public override async Task<RestResponse[]> GET(string url, uint maxAttempts = 1)
        {
            Checks.CheckForArgument(string.IsNullOrEmpty(url), nameof(url));
            return await Raw(_httpClient.GetAsync(url), GET_ALLOWED_STATUSES, maxAttempts);
        }

        public override async Task<RestResponse[]> POST(string url, string data, uint maxAttempts = 1)
        {
            Checks.CheckForArgument(string.IsNullOrEmpty(url), nameof(url));
            Checks.CheckForArgument(string.IsNullOrEmpty(data), nameof(data));
            // Need 'multipart/form-data' to send files
            using (var content = new StringContent(data, Encoding.UTF8, "application/json"))
                return await Raw(_httpClient.PostAsync(url, content), POST_ALLOWED_STATUSES, maxAttempts);
        }

        public override async Task<RestResponse[]> DELETE(string url, uint maxAttempts = 1)
        {
            Checks.CheckForArgument(string.IsNullOrEmpty(url), nameof(url));
            return await Raw(_httpClient.DeleteAsync(url), DELETE_ALLOWED_STATUSES, maxAttempts);
        }

        public override async Task<RestResponse[]> PATCH(string url, string data, uint maxAttempts = 1)
        {
            Checks.CheckForArgument(string.IsNullOrEmpty(url), nameof(url));
            Checks.CheckForArgument(string.IsNullOrEmpty(data), nameof(data));
            using (var content = new StringContent(data, Encoding.UTF8, "application/json"))
                return await Raw(_httpClient.PatchAsync(url, content), PATCH_ALLOWED_STATUSES, maxAttempts);
        }

        private async Task<RestResponse[]> Raw(Task<HttpResponseMessage> func, HttpStatusCode[] allowedStatuses, uint maxAttempts = 1)
        {
            Checks.CheckWebhookStatus(_restClient.Webhook.Status);
            Checks.CheckForNull(allowedStatuses, nameof(allowedStatuses));

            _locker.Wait();
            List<RestResponse> responses = new List<RestResponse>();
            uint currentAttimpts = 0;
            // Used to prevent calls if something went wrong
            bool forceStop = false;

            do
            {
                if (responses.Count != 0)
                    await _restClient.FollowRateLimit(responses.Last().RateLimit);

                HttpResponseMessage response = await func;
                RateLimitInfo rateLimitInfo = new RateLimitInfo(response.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault()));
                RestResponse restResponse = new RestResponse(response.StatusCode, rateLimitInfo, await response.Content.ReadAsStringAsync(), currentAttimpts);
                responses.Add(restResponse);

                // Processing the necessary status codes
                ProcessStatusCode(response.StatusCode, ref forceStop, allowedStatuses);
                Log(new LogContext(LogSensitivity.VERBOSE, $"[A {currentAttimpts}] [SC {(int)responses.Last().StatusCode}] [RLR {restResponse.RateLimit.Reset:yyyy-MM-dd HH:mm:ss.fff zzz}] [RLMW {restResponse.RateLimit.MustWait}] Request completed:{(restResponse.Content.Length != 0 ? string.Concat(Environment.NewLine, restResponse.Content) : " No content")}", _restClient.Webhook.Id));

            } while (!forceStop && (!allowedStatuses.Contains(responses.Last().StatusCode) && (maxAttempts > 0 ? ++currentAttimpts <= maxAttempts : true)));

            _locker.Release();
            return responses.ToArray();
        }
    }
}
