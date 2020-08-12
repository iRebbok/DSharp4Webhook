using DSharp4Webhook.Core;
using DSharp4Webhook.Extensions;
using System.IO;
using System.Net;
using System.Text;

namespace DSharp4Webhook.Rest
{
    /// <summary>
    ///     Structure of the server response containing the required data.
    /// </summary>
    public readonly struct RestResponse : IValidable
    {
        public HttpStatusCode StatusCode { get; }
        public RateLimitInfo RateLimit { get; }
        public byte[] Data { get; }
        public string? Content => (Data.Length != 0) ? Encoding.UTF8.GetString(Data) : null;
        public uint Attempts { get; }
        public bool IsSuccessful => IsValid() && (int)StatusCode < 300 && (int)StatusCode >= 200;

        public RestResponse(HttpStatusCode statusCode, RateLimitInfo rateLimit, Stream stream, uint attempts)
        {
            StatusCode = statusCode;
            RateLimit = rateLimit;
            Attempts = attempts;
            Data = stream.ReadAsByteArray();
        }

        public bool IsValid() => StatusCode != default && RateLimit != default && Data != default && Attempts != default;
    }
}
