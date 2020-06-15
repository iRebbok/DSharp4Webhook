using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DSharp4Webhook.Util.Extensions
{
    internal static class CollectionUtil
    {
        public static ReadOnlyCollection<T>? ToReadOnlyCollection<T>(this IList<T>? array)
        {
            if (array is null)
                return null;

            return new ReadOnlyCollection<T>(array);
        }
    }
}
