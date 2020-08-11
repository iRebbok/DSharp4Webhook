using DSharp4Webhook.Util;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DSharp4Webhook.Internal
{
    public static class DictionaryUtil
    {
        /// <summary>
        ///     Returns the size of these values.
        /// </summary>
        public static long SizeOf(this IDictionary<string, ReadOnlyCollection<byte>> source)
        {
            Checks.CheckForNull(source);

            long result = 0L;
            foreach (var pair in source)
                result += pair.Value.LongCount();
            return result;
        }
    }
}
