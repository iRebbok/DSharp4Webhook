using DSharp4Webhook.Buffers;
using System;
using System.Buffers;
using System.IO;

namespace DSharp4Webhook.Extensions
{
    public static class StreamExtensions
    {
        public const int BUFFER_SIZE = 2048;
        public const int SEGMENT_SIZE = BUFFER_SIZE * 2;

        /// <summary>
        ///     Reads the stream as an array of bytes.
        /// </summary>
        /// <param name="stream">
        ///     Source stream.
        /// </param>
        /// <returns>
        ///     An array segment of bytes that was taken from the pool.
        /// </returns>
        /// <remarks><inheritdoc cref="ArraySegmentExtensions.Write{T}(ref ArraySegment{T}, T[], int, int)" /></remarks>
        public static ArraySegment<byte> ReadAsByteSegment(this Stream stream)
        {
            var result = new ArraySegment<byte>(ArrayPool<byte>.Shared.Rent(SEGMENT_SIZE), 0, 0);
            var buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);

            int readed;
            while ((readed = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ArraySegmentExtensions.Write(ref result, buffer, 0, readed);
            }

            ArrayPool<byte>.Shared.Return(buffer, true);
            return result;
        }

        /// <inheritdoc cref="ReadAsByteSegment(Stream)"/>
        /// <remarks>><inheritdoc cref="ArraySegmentExtensions.Write{T}(ref LongArraySegment{T}, T[], long, long)" /></remarks>
        public static LongArraySegment<byte> ReadAsLongByteSegment(this Stream stream)
        {
            var result = new LongArraySegment<byte>(LongArrayPool<byte>.Rent(SEGMENT_SIZE), 0, 0);
            var buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);

            int readed;
            while ((readed = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ArraySegmentExtensions.Write(ref result, buffer, 0, readed);
            }

            ArrayPool<byte>.Shared.Return(buffer, true);
            return result;
        }
    }
}
