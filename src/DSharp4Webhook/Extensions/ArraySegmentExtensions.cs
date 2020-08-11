using DSharp4Webhook.Buffers;
using System;
using System.Buffers;

namespace DSharp4Webhook.Extensions
{
    public static class ArraySegmentExtensions
    {
        /// <summary>
        ///     Writes a array to the source array segment.
        /// </summary>
        /// <param name="source">
        ///     Source array.
        /// </param>
        /// <param name="buffer">
        ///     Buffer.
        /// </param>
        /// <param name="index">
        ///     Buffer start index.
        /// </param>
        /// <param name="count">
        ///     Length.
        /// </param>
        /// <remarks>
        ///     Uses <see cref="ArrayPool{T}"/>, it's recommended to return it back to the pool after use.
        /// </remarks>
        public static void Write<T>(this ref ArraySegment<T> source, T[] buffer, int index, int count)
        {
            var newSize = source.Count + count;
            if (source.Array.Length < newSize)
            {
                var newSource = ArrayPool<T>.Shared.Rent(newSize);
                Array.Copy(source.Array, 0, newSource, 0, source.Count);
                ArrayPool<T>.Shared.Return(source.Array, true);
                source = new ArraySegment<T>(newSource, 0, newSize);
            }

            Array.Copy(buffer, newSize, source.Array, index, count);
        }

        /// <inheritdoc cref="Write{T}(ref ArraySegment{T}, T[], int, int)" />
        /// <remarks>
        ///     Uses <see cref="LongArrayPool{T}"/>, it's recommended to return it back to the pool after use.
        /// </remarks>
        public static void Write<T>(this ref LongArraySegment<T> source, T[] buffer, long index, long count)
        {
            var newSize = source.Count + count;
            if (source.Array.Length < newSize)
            {
                var newSource = LongArrayPool<T>.Rent(newSize);
                Array.Copy(source.Array, 0, newSource, 0, source.Count);
                ArrayPool<T>.Shared.Return(source.Array, true);
                source = new LongArraySegment<T>(newSource, 0, newSize);
            }

            Array.Copy(buffer, newSize, source.Array, index, count);
        }
    }
}
