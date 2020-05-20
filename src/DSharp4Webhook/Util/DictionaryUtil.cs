using System.Collections.Generic;

namespace DSharp4Webhook.Util
{
    public static class DictionaryUtil
    {
        /// <summary>
        ///     Returns the size of these values.
        /// </summary>
        public static long SizeOf<T>(this IEnumerable<KeyValuePair<T, byte[]>> source)
        {
            Checks.CheckForNull(source);

            long result = 0L;
            foreach (var pair in source)
                result += pair.Value.LongLength;
            return result;
        }
    }
}
