using System;
using System.Collections.Generic;

namespace DSharp4Webhook.Buffers
{
    public static class LongArrayPool<T>
    {
        private static readonly List<T[]> _pool = new List<T[]>(100);

        public static T[] Rent(long capacity)
        {
            lock (_pool)
            {
                for (int z = 0; z < _pool.Count; z++)
                {
                    var arr = _pool[z];
                    if (arr.LongLength <= capacity)
                    {
                        _pool.RemoveAt(z);
                        return arr;
                    }
                }

                return new T[capacity];
            }
        }

        public static void Recycle(T[] arr)
        {
            if (arr is null)
                throw new ArgumentNullException(nameof(arr));

            lock (_pool)
            {
                Array.Clear(arr, 0, arr.Length);
                _pool.Add(arr);
            }
        }
    }
}
