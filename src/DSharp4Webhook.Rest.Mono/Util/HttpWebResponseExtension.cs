using System;
using System.Net;
using System.Threading.Tasks;

namespace DSharp4Webhook.Rest.Mono.Util
{
    public static class HttpWebResponseExtension
    {
        /// <summary>
        ///     Very useful thing, no need to get twisted in the code.
        /// </summary>
        public static async Task<HttpWebResponse> GetResponseNoException(this HttpWebRequest req)
        {
            try
            {
                if (await req.GetResponseAsync().ConfigureAwait(false) is HttpWebResponse webResponse)
                    return webResponse;
            }
            catch (WebException we) when (we.Response is HttpWebResponse resp)
            {
                return resp;
            }

            throw new InvalidOperationException();
        }
    }
}
