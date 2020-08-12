using DSharp4Webhook.Core;
using DSharp4Webhook.InternalExceptions;
using DSharp4Webhook.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DSharp4Webhook.Util
{
    public static class Contract
    {
        /// <summary>
        ///     Check webhook for broken.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The webhook status indicates that it's broken.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.IL)]
        public static void AssertWebhookIsNotBroken(IWebhook webhook)
        {
            if (webhook.Status == WebhookStatus.NOT_EXIST)
                throw new InvalidOperationException("Attempt to interact with a nonexistent webhook");
        }

        /// <summary>
        ///     Asserts that the object isn't null.
        /// </summary>
        /// <param name="instance">
        ///     Object instance.
        /// </param>
        /// <param name="paramName">
        ///     The name of the parameter that caused the current exception.
        /// </param>
        /// <param name="message">
        ///     The error message that explains the reason for the exception.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="instance"/> is null.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.IL)]
        public static void AssertNotNull<T>(T? instance, string? paramName = null, string? message = null)
            where T : class
        {
            if (instance is null)
                throw new ArgumentNullException(paramName, message);
        }

        /// <summary>
        ///     Asserts that the value isn't true.
        /// </summary>
        /// <param name="value">
        ///     Argument check result.
        /// </param>
        /// <param name="paramName">
        ///     The name of the parameter that caused the current exception.
        /// </param>
        /// <param name="message">
        ///     The error message that explains the reason for the exception.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     <paramref name="value"/> is true.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.IL)]
        public static void AssertArgumentNotTrue(bool value, string? paramName = null, string? message = null)
        {
            if (value)
                throw new ArgumentException(message, paramName);
        }

        /// <summary><inheritdoc cref="AssertSafeBoundsSafe(int, int, int)" /></summary>
        /// <param name="value"><inheritdoc cref="AssertSafeBoundsSafe(int, int, int)" /></param>
        /// <param name="lowest"><inheritdoc cref="AssertSafeBoundsSafe(int, int, int)" /></param>
        /// <param name="highest"><inheritdoc cref="AssertSafeBoundsSafe(int, int, int)" /></param>
        /// <param name="paramName">
        ///     The name of the parameter that caused the current exception.
        /// </param>
        /// <param name="message">
        ///     The error message that explains the reason for the exception.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException"><inheritdoc cref="AssertSafeBounds(int, int, int, string?, string?)" /></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.IL)]
        public static void AssertSafeBounds(string value, int lowest, int highest, string? paramName = null, string? message = null)
            => AssertSafeBounds(value.Length, lowest, highest, paramName, message);

        /// <summary><inheritdoc cref="AssertSafeBoundsSafe(int, int, int)" /></summary>
        /// <param name="value"><inheritdoc cref="AssertSafeBoundsSafe(int, int, int)" /></param>
        /// <param name="lowest"><inheritdoc cref="AssertSafeBoundsSafe(int, int, int)" /></param>
        /// <param name="highest"><inheritdoc cref="AssertSafeBoundsSafe(int, int, int)" /></param>
        /// <param name="paramName">
        ///     The name of the parameter that caused the current exception.
        /// </param>
        /// <param name="message">
        ///     The error message that explains the reason for the exception.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Value is out of bounds.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.IL)]
        public static void AssertSafeBounds(int value, int lowest, int highest, string? paramName = null, string? message = null)
        {
            if (!AssertSafeBoundsSafe(value, lowest, highest))
                throw new ArgumentOutOfRangeException(paramName, message);
        }

        /// <summary>
        ///     Asserts that the value is within the bounds.
        /// </summary>
        /// <param name="value">
        ///     Source value.
        /// </param>
        /// <param name="lowest">
        ///     Lowest border value.
        /// </param>
        /// <param name="highest">
        ///     Highest border value.
        /// </param>
        /// <returns>
        ///     true if the value fits the bounds; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.IL)]
        public static bool AssertSafeBoundsSafe(int value, int lowest, int highest) => value >= lowest && value <= highest;

        /// <summary>
        ///     Asserts that the serialization type matches the requested type.
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
        [MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.IL)]
        public static void AssertRequiredSerizationType(SerializationContext context, SerializationType requireType)
        {
            if (context.Type != requireType)
                throw new InvalidOperationException($"The current operation needs the {requireType} serialization type, not the {context.Type}");
        }

        /// <inheritdoc cref="AssertNotOversizeSafe(decimal)" />
        /// <exception cref="SizeOutOfRangeException">
        ///     The size is too large.
        /// </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.IL)]
        public static void AssertNotOversize(List<FileEntry> fileEntries)
        {
            if (!AssertNotOversizeSafe(fileEntries))
                throw new SizeOutOfRangeException();
        }

        /// <inheritdoc cref="AssertNotOversizeSafe(decimal)" />
        [MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.IL)]
        public static bool AssertNotOversizeSafe(List<FileEntry> fileEntries)
        {
            var size = 0M;
            unchecked
            {
                foreach (var entry in fileEntries)
                    size += entry.Content.LongLength;
            }
            return AssertNotOversizeSafe(size);
        }

        /// <inheritdoc cref="AssertNotOversizeSafe(byte[])" />
        [MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.IL)]
        public static bool AssertNotOversizeSafe(FileEntry fileEntry) => AssertNotOversizeSafe(fileEntry.Content);

        /// <inheritdoc cref="AssertNotOversizeSafe(decimal)" />
        public static bool AssertNotOversizeSafe(this byte[] arr) => AssertNotOversizeSafe(arr.LongLength);

        /// <summary>
        ///     Asserts that the value doesn't exceed the size of the attachment.
        /// </summary>
        /// <returns>
        ///     false if doesn't exceed the size; otherwise true.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining, MethodCodeType = MethodCodeType.IL)]
        public static bool AssertNotOversizeSafe(decimal length) => length >= WebhookProvider.MAX_ATTACHMENTS_SIZE;
    }
}
