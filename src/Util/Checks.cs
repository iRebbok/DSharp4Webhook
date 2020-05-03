using System;
using System.Linq;

namespace DSharp4Webhook.Util
{
    /// <summary>
    ///     Includes various checks to protect against duplication.
    /// </summary>
    public static class Checks
    {
        /// <summary>
        ///     Ckecks the bounds and if they are exceeded or meet it causes an exception.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     If the length exceed or meet their bounds.
        /// </exception>
        public static void CheckBounds(string paramName, string message, int safeLength, int length, params int[] length1)
        {
            if (CheckBoundsSafe(safeLength, length, length1))
                throw new ArgumentOutOfRangeException(paramName, message);
        }

        /// <summary>
        ///     Checks the bounds safe.
        /// </summary>
        /// <returns>
        ///     true if the length is equal to or exceeds the bounds,
        ///     otherwise false.
        /// </returns>
        public static bool CheckBoundsSafe(int safeLength, int length, params int[] length1)
        {
            long result = length;
            if (length1 != null)
                result += length1.Sum(a => (long)a);
            return result >= safeLength;
        }
    }
}
