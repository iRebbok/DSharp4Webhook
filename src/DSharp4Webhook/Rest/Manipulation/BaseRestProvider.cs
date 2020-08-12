using DSharp4Webhook.Core;
using DSharp4Webhook.Serialization;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;
using System.Net;

namespace DSharp4Webhook.Rest.Manipulation
{
    public abstract class BaseRestProvider : IDisposable
    {
        protected readonly IWebhook _webhook;

        protected BaseRestProvider(IWebhook webhook)
        {
            Contract.AssertNotNull(webhook, nameof(webhook));

            _webhook = webhook;
        }

        public abstract IEnumerable<RestResponse> POST(string url, SerializationContext data, RestSettings restSettings);

        public abstract IEnumerable<RestResponse> GET(string url, RestSettings restSettings);

        public abstract IEnumerable<RestResponse> DELETE(string url, RestSettings restSettings);

        public abstract IEnumerable<RestResponse> PATCH(string url, SerializationContext data, RestSettings restSettings);

        /// <summary>
        ///     Wrapper for processing returned status codes.
        /// </summary>
        protected void ProcessStatusCode(HttpStatusCode statusCode, ref bool forceStop)
        {
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    _webhook.Status = WebhookStatus.NOT_EXIST;
                    forceStop = true;
                    _webhook.Dispose();
                    break;
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.RequestEntityTooLarge:
                    forceStop = true;
                    break;
            }

            if (!forceStop && _webhook.Status == WebhookStatus.NOT_CHECKED)
            {
                _webhook.Status = WebhookStatus.EXIST;
            }
        }

        public abstract void Dispose();
    }
}
