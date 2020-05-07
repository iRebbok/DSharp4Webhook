using DSharp4Webhook.Core;
using DSharp4Webhook.Logging;
using DSharp4Webhook.Rest.Entities;
using DSharp4Webhook.Util;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DSharp4Webhook.Rest.Manipulation
{
    public abstract class BaseRestProvider
    {
        protected static readonly HttpStatusCode[] POST_ALLOWED_STATUSES = new HttpStatusCode[1] { HttpStatusCode.NoContent };
        protected static readonly HttpStatusCode[] GET_ALLOWED_STATUSES = new HttpStatusCode[1] { HttpStatusCode.OK };

        protected readonly RestClient _restClient;
        protected readonly SemaphoreSlim _locker;

        protected BaseRestProvider(RestClient restClient, SemaphoreSlim locker)
        {
            Checks.CheckForNull(restClient, nameof(restClient));
            Checks.CheckForNull(locker, nameof(locker));

            _restClient = restClient;
            _locker = locker;
        }

        public abstract Task<RestResponse[]> POST(string url, string data, uint maxAttempts = 1);

        public abstract Task<RestResponse[]> GET(string url, uint maxAttempts = 1);

        /// <remarks>
        ///     Wrapper for processing returned status codes.
        /// </remarks>
        /// <param name="allowedStatuses">
        ///     Allowed statuses that are considered successful requests.
        /// </param>
        protected void ProcessStatusCode(HttpStatusCode statusCode, ref bool forceStop, HttpStatusCode[] allowedStatuses)
        {
            Checks.CheckForNull(allowedStatuses, nameof(allowedStatuses));

            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    _restClient.Webhook.Status = WebhookStatus.NOT_EXISTING;
                    _restClient.Webhook.Provider?.Log(new LogContext(LogSensitivity.ERROR, "A REST request returned 404, the webhack does not exist, and we are deleting it...", _restClient.Webhook.Id));
                    forceStop = true;
                    _restClient.Webhook.Dispose();
                    break;
                case HttpStatusCode.BadRequest:
                    _restClient.Webhook.Provider?.Log(new LogContext(LogSensitivity.ERROR, "A REST request returnet 400, something went wrong...", _restClient.Webhook.Id));
                    forceStop = true;
                    break;
            }

            if (allowedStatuses.Contains(statusCode))
            {
                if (_restClient.Webhook.Status != WebhookStatus.EXISTING)
                {
                    _restClient.Webhook.Status = WebhookStatus.EXISTING;
                    _restClient.Webhook.Provider?.Log(new LogContext(LogSensitivity.INFO, $"Webhook confirmed its status", _restClient.Webhook.Id));
                }
            }
        }

        protected void Log(LogContext context)
        {
            _restClient.Webhook.Provider?.Log(context);
        }
    }
}
