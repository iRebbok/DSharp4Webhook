using System.Net;

namespace DSharp4Webhook.Rest
{
    /// <summary>
    ///     Structure of the server response containing the required data.
    /// </summary>
    public struct RestResponse
    {
        public HttpStatusCode StatusCode { get; }
        public RateLimitInfo RateLimit { get; }
        public string Content { get; }
        public uint Attempts { get; }

        public RestResponse(HttpStatusCode code, RateLimitInfo rateLimit, string content, uint attempts)
        {
            StatusCode = code;
            RateLimit = rateLimit;
            Content = content;
            Attempts = attempts;
        }
    }
}
