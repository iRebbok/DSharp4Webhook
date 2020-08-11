using DSharp4Webhook.Extensions;
using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Text;

namespace DSharp4Webhook.Rest
{
    /// <summary>
    ///     Structure of the server response containing the required data.
    /// </summary>
    public readonly struct RestResponse : IDisposable
    {
        public HttpStatusCode StatusCode { get; }
        public RateLimitInfo RateLimit { get; }
        public ArraySegment<byte> Data { get; }
        public string? Content => (Data.Count != 0) ? Encoding.UTF8.GetString(Data.Array) : null;
        public uint Attempts { get; }

        public RestResponse(HttpStatusCode statusCode, RateLimitInfo rateLimit, Stream stream, uint attempts)
        {
            StatusCode = statusCode;
            RateLimit = rateLimit;
            Attempts = attempts;
            Data = stream.ReadAsByteSegment();
        }

        /// <summary>
        ///     Returns an array to the pool.
        /// </summary>
        public void Dispose()
        {
            ArrayPool<byte>.Shared.Return(Data.Array, true);
        }
    }
}
