using DSharp4Webhook.Core;
using DSharp4Webhook.Rest.Manipulation;
using DSharp4Webhook.Serialization;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DSharp4Webhook.Rest
{
    public sealed class DefaultProvider : BaseRestProvider
    {
        private readonly HttpClient _httpClient;

        // This type of request is not available in the .NET Framework 4.7.1.
        private static readonly HttpMethod PATCHMethod = new HttpMethod("PATH");

        /// <summary>
        ///     Sets the given provider is the default.
        /// </summary>
        public static void SetupAsDefault()
        {
            RestProviderLoader.SetProviderType(typeof(DefaultProvider));
        }

        public DefaultProvider(IWebhook webhook) : base(webhook)
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"DSharp4Webhook ({WebhookProvider.LibraryUrl}, {WebhookProvider.LibraryVersion})");
            _httpClient.DefaultRequestHeaders.Add("X-RateLimit-Precision", "millisecond");
        }

        public override IEnumerable<RestResponse> GET(string url, RestSettings restSettings)
        {
            Contract.AssertArgumentNotTrue(string.IsNullOrEmpty(url), nameof(url));
            return Raw(_httpClient.GetAsync(url), restSettings);
        }

        public override IEnumerable<RestResponse> POST(string url, SerializationContext data, RestSettings restSettings)
        {
            Contract.AssertArgumentNotTrue(string.IsNullOrEmpty(url), nameof(url));

            using HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);
            PrepareContent(requestMessage, data);
            return Raw(_httpClient.SendAsync(requestMessage), restSettings);
        }

        public override IEnumerable<RestResponse> DELETE(string url, RestSettings restSettings)
        {
            Contract.AssertArgumentNotTrue(string.IsNullOrEmpty(url), nameof(url));
            return Raw(_httpClient.DeleteAsync(url), restSettings);
        }

        public override IEnumerable<RestResponse> PATCH(string url, SerializationContext data, RestSettings restSettings)
        {
            Contract.AssertArgumentNotTrue(string.IsNullOrEmpty(url), nameof(url));
            Contract.AssertRequiredSerizationType(data, SerializationType.APPLICATION_JSON);

            using HttpRequestMessage requestMessage = new HttpRequestMessage(PATCHMethod, url);
            PrepareContent(requestMessage, data);
            return Raw(_httpClient.SendAsync(requestMessage), restSettings);
        }

        private IEnumerable<RestResponse> Raw(Task<HttpResponseMessage> func, RestSettings restSettings)
        {
            Contract.EnsureWebhookIsNotBroken(_webhook.Status);

            var currentAttempts = 0U;
            // Used to prevent calls if something went wrong
            var forceStop = false;
            var lastResponse = default(RestResponse);

            do
            {
                if (lastResponse.IsValid())
                    _webhook.ActionManager.FollowRateLimit(lastResponse.RateLimit).ConfigureAwait(false).GetAwaiter().GetResult();

                using var response = func.ConfigureAwait(false).GetAwaiter().GetResult();
                var rateLimitInfo = RateLimitInfo.Get(response.Headers);
                using var responseStream = response.Content.ReadAsStreamAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                lastResponse = new RestResponse(response.StatusCode, rateLimitInfo, responseStream, currentAttempts);

                // Processing the necessary status codes
                ProcessStatusCode(response.StatusCode, ref forceStop);

                yield return lastResponse;

            } while (!forceStop && !lastResponse.IsSuccessful && (restSettings.Attempts == 0 || ++currentAttempts <= restSettings.Attempts));
        }

        private readonly static MediaTypeHeaderValue _applicationJsonTypeHeader = MediaTypeHeaderValue.Parse(SerializeTypeConverter.Convert(SerializationType.APPLICATION_JSON));

        private void PrepareContent(HttpRequestMessage requestMessage, SerializationContext data)
        {
            switch (data.Type)
            {
                case SerializationType.APPLICATION_JSON:
                    {
                        requestMessage.Content = new ByteArrayContent(data.Content);
                        requestMessage.Content.Headers.ContentType = _applicationJsonTypeHeader;
                        break;
                    }

                case SerializationType.MULTIPART_FORM_DATA:
                    {
                        var multipartContent = new MultipartFormDataContent();
                        if (!(data.Attachments is null) && data.Attachments.Length != 0)
                        {
                            for (var z = 0; z < data.Attachments.Length; z++)
                            {
                                var entry = data.Attachments[z];
                                multipartContent.Add(new ByteArrayContent(entry.Content), $"file{z}", entry.FileName);
                            }
                        }

                        if (!(data.Content is null))
                            multipartContent.Add(new ByteArrayContent(data.Content), "payload_json");

                        requestMessage.Content = multipartContent;
                        // it doesn't seem to be necessary, it works automatically
                        //requestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(SerializeTypeConverter.Convert(SerializeType.MULTIPART_FROM_DATA));
                        break;
                    }
            }
        }

        ~DefaultProvider() => Dispose(true);

        public override void Dispose() => Dispose(false);

        private void Dispose(bool disposing)
        {
            _httpClient.Dispose();
            if (!disposing)
                GC.SuppressFinalize(this);
        }
    }
}
