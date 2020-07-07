using DSharp4Webhook.Core;
using DSharp4Webhook.Logging;
using DSharp4Webhook.Serialization;
using DSharp4Webhook.Util;
using DSharp4Webhook.Util.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DSharp4Webhook.Rest.Manipulation
{
    public abstract class BaseRestProvider
    {
        // When we send a file, we get the status code 200 with detailed information in response, also when 'wait=true' as a query parameter
        public static readonly IReadOnlyCollection<HttpStatusCode> POST_ALLOWED_STATUSES = new HttpStatusCode[2] { HttpStatusCode.NoContent, HttpStatusCode.OK }.ToReadOnlyCollection()!;
        public static readonly IReadOnlyCollection<HttpStatusCode> GET_ALLOWED_STATUSES = new HttpStatusCode[1] { HttpStatusCode.OK }.ToReadOnlyCollection()!;
        public static readonly IReadOnlyCollection<HttpStatusCode> DELETE_ALLOWED_STATUSES = new HttpStatusCode[1] { HttpStatusCode.NoContent }.ToReadOnlyCollection()!;
        public static readonly IReadOnlyCollection<HttpStatusCode> PATCH_ALLOWED_STATUSES = new HttpStatusCode[1] { HttpStatusCode.OK }.ToReadOnlyCollection()!;

        protected readonly IWebhook _webhook;

        protected BaseRestProvider(IWebhook webhook)
        {
            Checks.CheckForNull(webhook, nameof(webhook));

            _webhook = webhook;
        }

        public abstract Task<RestResponse[]> POST(string url, SerializeContext data, RestSettings restSettings);

        public abstract Task<RestResponse[]> GET(string url, RestSettings restSettings);

        public abstract Task<RestResponse[]> DELETE(string url, RestSettings restSettings);

        public abstract Task<RestResponse[]> PATCH(string url, SerializeContext data, RestSettings restSettings);

        /// <remarks>
        ///     Wrapper for processing returned status codes.
        /// </remarks>
        /// <param name="allowedStatuses">
        ///     Allowed statuses that are considered successful requests.
        /// </param>
        protected void ProcessStatusCode(HttpStatusCode statusCode, ref bool forceStop, IReadOnlyCollection<HttpStatusCode> allowedStatuses)
        {
            Checks.CheckForNull(allowedStatuses, nameof(allowedStatuses));

            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    _webhook.Status = WebhookStatus.NOT_EXISTING;
                    Log(new LogContext(LogSensitivity.ERROR, "A REST request returned 404, the webhack does not exist, and we are deleting it...", _webhook.Id));
                    forceStop = true;
                    _webhook.Dispose();
                    break;
                case HttpStatusCode.BadRequest:
                    Log(new LogContext(LogSensitivity.ERROR, "A REST request returnet 400, something went wrong...", _webhook.Id));
                    forceStop = true;
                    break;
                case HttpStatusCode.RequestEntityTooLarge:
                    Log(new LogContext(LogSensitivity.WARN, "A REST request returned 413, you sent too much data", _webhook.Id));
                    forceStop = true;
                    break;
            }

            if (allowedStatuses.Contains(statusCode))
            {
                if (_webhook.Status != WebhookStatus.EXISTING)
                {
                    _webhook.Status = WebhookStatus.EXISTING;
                    Log(new LogContext(LogSensitivity.INFO, $"Webhook confirmed its status", _webhook.Id));
                }
            }
        }

        protected void Log(LogContext context)
        {
            //todo: webhook logs
        }
    }
}
