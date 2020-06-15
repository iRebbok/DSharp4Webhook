using DSharp4Webhook.Core;
using DSharp4Webhook.Exception;
using DSharp4Webhook.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DSharp4Webhook.Util
{
    /// <summary>
    ///     Includes various checks to protect against duplication.
    /// </summary>
    public static class Checks
    {
        /// <summary>
        ///     Checks the bounds and if they are exceeded or meet it causes an exception.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     If the length exceed or meet their bounds.
        /// </exception>
        public static void CheckBounds(string? paramName, string? message, int safeLength, int length, params int[] length1)
        {
            if (CheckBoundsSafe(safeLength, length, length1))
                throw new ArgumentOutOfRangeException(paramName, message);
        }

        /// <summary>
        ///     Checks the bounds, and if they are belittled or meet, it causes an exception.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     If the length exceed or meet their bounds.
        /// </exception>
        public static void CheckBoundsUnderside(string? paramName, string? message, int safeLength, int length, params int[] length1)
        {
            if (CheckBoundsUndersideSafe(safeLength, length, length1))
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
            if (!(length1 is null))
                result += length1.Sum(a => (long)a);
            return result >= safeLength;
        }

        public static bool CheckBoundsUndersideSafe(int safeLength, int length, params int[] length1)
        {
            return !CheckBoundsSafe(safeLength, length, length1);
        }

        /// <summary>
        ///     Checks the status of the webhook.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     If the state is not suitable for interaction.
        /// </exception>
        public static void CheckWebhookStatus(WebhookStatus status)
        {
            if (status == WebhookStatus.NOT_EXISTING)
                throw new InvalidOperationException("Attempt to interact with a nonexistent webhook");
        }

        /// <summary>
        ///     Checks the object for null.
        /// </summary>
        /// <param name="message">
        ///     Customized message.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     If the specified object is null.
        /// </exception>
        public static void CheckForNull<T>(T instance, string? paramName = null, string? message = null)
            where T : class
        {
            if (instance is null)
                throw new ArgumentNullException(paramName ?? typeof(T).Name, message ?? $"The object {typeof(T).Name} can't be null");
        }

        /// <summary>
        ///     Checks for an argument.
        /// </summary>
        /// <param name="boolean">
        ///     Specifies whether to throw an exception or not.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If boolean is true.
        /// </exception>
        public static void CheckForArgument(bool boolean, string? paramName = null, string? message = null)
        {
            if (boolean)
                throw new ArgumentException(message, paramName);
        }

        /// <summary>
        ///     Checks for compliance with the serialization type.
        /// </summary>
        /// <param name="context">
        ///     The context of serialization.
        /// </param>
        /// <param name="requireType">
        ///     Required type for serialization.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     The serialization type is not suitable.
        /// </exception>
        public static void CheckForSerializeType(SerializeContext context, SerializeType requireType)
        {
            if (context.Type != requireType)
                throw new InvalidOperationException($"The current operation needs the {requireType} serialization type, not the {context.Type}");
        }

        /// <summary>
        ///     Checking for attachments.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     The number of attachments exceeds the limit.
        /// </exception>
        /// <exception cref="SizeOutOfRangeException">
        ///     The size of attachments exceeds the limit.
        /// </exception>
        public static void CheckForAttachments(IDictionary<string, ReadOnlyCollection<byte>> source)
        {
            if (source is null)
                return;

            if (source.Count() > WebhookProvider.MAX_ATTACHMENTS)
                throw new ArgumentOutOfRangeException();

            if (source.SizeOf() > WebhookProvider.MAX_ATTACHMENTS_SIZE)
                throw new SizeOutOfRangeException();
        }
    }
}
