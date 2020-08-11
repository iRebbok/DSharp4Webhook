using System;

namespace DSharp4Webhook.InternalExceptions
{
    /// <summary>
    ///     Exceptions that occur when the allowed limit for attachments is exceeded.
    /// </summary>
    [Serializable]
    public sealed class SizeOutOfRangeException : Exception
    {
        public SizeOutOfRangeException(string message) : base(message) { }

        public SizeOutOfRangeException(string message, Exception innerException) : base(message, innerException) { }

        public SizeOutOfRangeException() { }
    }
}
