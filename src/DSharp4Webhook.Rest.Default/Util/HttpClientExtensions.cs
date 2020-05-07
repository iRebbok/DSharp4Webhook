using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DSharp4Webhook.Util
{
    public static class HttpClientExtensions
    {
        /// <summary>
        ///     Patch request to url.
        /// </summary>
        /// <remarks>
        ///     This type of request is not available in the .NET Framework 4.7.1.
        /// </remarks>
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string url, HttpContent content)
        {
            var method = new HttpMethod("PATCH");

            var request = new HttpRequestMessage(method, url)
            {
                Content = content
            };

            HttpResponseMessage response = new HttpResponseMessage();
            // We expect no more than 60 seconds
            CancellationToken cancellationToken = new CancellationTokenSource(60).Token;
            try
            {
                response = await client.SendAsync(request, cancellationToken);
            }
            catch (TaskCanceledException e) { }

            return response;
        }
    }
}
