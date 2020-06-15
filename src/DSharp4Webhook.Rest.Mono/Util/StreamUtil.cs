using System.IO;
using System.Text;

namespace DSharp4Webhook.Rest.Mono.Util
{
    public static class StreamUtil
    {
        /// <summary>
        ///     Writes data to a stream.
        /// </summary>
        public static void Write(Stream source, string content, Encoding encoding)
        {
            if (encoding is null)
                encoding = Encoding.UTF8;

            byte[] buffer = encoding.GetBytes(content);
            source.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        ///     Reads data from the stream.
        /// </summary>
        public static string Read(Stream source)
        {
            using StreamReader reader = new StreamReader(source);
            return reader.ReadToEnd();
        }
    }
}
