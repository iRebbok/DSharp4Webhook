using System.Net;
using System.Text;

namespace DSharp4Webhook.Rest
{
    /// <summary>
    ///     Structure of the server response containing the required data.
    /// </summary>
    public struct RestResponse
    {
        public HttpStatusCode StatusCode { get; }
        public RateLimitInfo RateLimit { get; }
        public byte[] Data { get; }
        public string Content { get => Encoding.UTF8.GetString(Data); }
        public uint Attempts { get; }

        public RestResponse(HttpWebResponse webResponse, RateLimitInfo rateLimit, uint attempts)
        {
            StatusCode = webResponse.StatusCode;
            RateLimit = rateLimit;
            Attempts = attempts;

            using (var stream = webResponse.GetResponseStream())
            {
                Data = new byte[stream.Length];
                stream.Read(Data, 0, Data.Length);
            }
        }

        public RestResponse(HttpStatusCode statusCode, RateLimitInfo rateLimit, byte[] data, uint attempts)
        {
            StatusCode = statusCode;
            RateLimit = rateLimit;
            Data = data;
            Attempts = attempts;
        }
    }
}
