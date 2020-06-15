using System.IO;
using System.Net;
using System.Text;

namespace DSharp4Webhook.Rest
{
    /// <summary>
    ///     Structure of the server response containing the required data.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public readonly struct RestResponse
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        public HttpStatusCode StatusCode { get; }
        public RateLimitInfo RateLimit { get; }
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[]? Data { get; }
#pragma warning restore CA1819 // Properties should not return arrays
        public string? Content { get => !(Data is null) ? Encoding.UTF8.GetString(Data) : null; }
        public uint Attempts { get; }

        public RestResponse(HttpWebResponse webResponse, RateLimitInfo rateLimit, uint attempts)
        {
            StatusCode = webResponse.StatusCode;
            RateLimit = rateLimit;
            Attempts = attempts;

            Data = null;
            Stream? stream = null;
            try
            {
                stream = webResponse.GetResponseStream();
                if (stream.CanRead)
                {
                    using var memoryStream = new MemoryStream();
                    var buffer = new byte[2048];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytesRead);
                    }
                    Data = memoryStream.ToArray();
                }
            }
            catch (System.Exception) { throw; }
            finally
            {
                stream?.Dispose();
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
