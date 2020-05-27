using System;

namespace DSharp4Webhook.Exception
{
    /// <summary>
    ///     Exceptions that occur when the allowed limit for attachments is exceeded.
    /// </summary>
    [Serializable]
#pragma warning disable CA2229 // Implement serialization constructors
    public sealed class SizeOutOfRangeException : System.Exception
#pragma warning restore CA2229 // Implement serialization constructors
    {
        public SizeOutOfRangeException(string message) : base(message) { }

        public SizeOutOfRangeException(string message, System.Exception innerException) : base(message, innerException) { }

        public SizeOutOfRangeException() { }
    }
}
