using System.Buffers;
using System.IO;

namespace DSharp4Webhook.Extensions
{
    public static class StreamExtensions
    {
        public const int BUFFER_SIZE = 2048;

        /// <summary>
        ///     Reads the stream as an array of bytes.
        /// </summary>
        /// <param name="stream">
        ///     Source stream.
        /// </param>
        /// <returns>
        ///     An array segment of bytes that was taken from the pool.
        /// </returns>
        public static byte[] ReadAsByteArray(this Stream stream)
        {
            using var memoryStream = new MemoryStream();
            var buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);

            int readed;
            while ((readed = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                memoryStream.Write(buffer, 0, readed);
            }

            ArrayPool<byte>.Shared.Return(buffer, true);
            return memoryStream.ToArray();
        }
    }
}
