using System.Collections.Generic;
using System.Net;

namespace DSharp4Webhook.Rest.Mono.Util
{
    public static class HttpWebExtension
    {
        /// <summary>
        ///     Very useful thing, no need to get twisted in the code.
        /// </summary>
        public static HttpWebResponse GetResponseNoException(this HttpWebRequest req)
        {
            try
            {
#pragma warning disable CS8603 // Possible null reference return.
                return req.GetResponse() as HttpWebResponse;
#pragma warning restore CS8603 // Possible null reference return.
            }
            catch (WebException we)
            {
                if (we.Response is HttpWebResponse resp)
                    return resp;
                else
                    throw;
            }
        }

        public static Dictionary<string, string> GetAsDictionary(this WebHeaderCollection collection)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (string headerKey in collection)
                result.Add(headerKey, collection.Get(headerKey));
            return result;
        }
    }
}
