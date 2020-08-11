using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DSharp4Webhook.Buffers
{
    public static class DictionaryStringPool
    {
        private static readonly ConcurrentBag<Dictionary<string, string>> _pool = new ConcurrentBag<Dictionary<string, string>>();

        public static Dictionary<string, string> Rent()
        {
            if (_pool.TryTake(out var value))
                return value;

            return new Dictionary<string, string>(50);
        }

        public static void Recycle(Dictionary<string, string> value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            value.Clear();
            _pool.Add(value);
        }
    }
}
