using System.Collections.Generic;

namespace DSharp4Webhook.Rest.Mono.Util
{
    public static class KeyValuePairExtensions
    {
        /// <summary>
        ///     Deconstructs a <see cref="KeyValuePair{TKey,TValue}"/> to a tuple. Intended for use by compiler.
        /// </summary>
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> @this, out T1 key, out T2 value)
        {
            key = @this.Key;
            value = @this.Value;
        }
    }
}
