using System.IO;
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
        public string Content { get => !(Data is null) ? Encoding.UTF8.GetString(Data) : null; }
        public uint Attempts { get; }

        public RestResponse(HttpWebResponse webResponse, RateLimitInfo rateLimit, uint attempts)
        {
            StatusCode = webResponse.StatusCode;
            RateLimit = rateLimit;
            Attempts = attempts;

            Data = null;
            Stream stream = null;
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
            // When using using we will not be able to track the exception
            catch (System.Exception)
            {
                // todo: logs
            }
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
