using System;

namespace DSharp4Webhook.Logging
{
    public readonly struct LogEntry
    {
        /// <summary>
        ///     The sensitivity of the log.
        /// </summary>
        public LogSensitivity Sensitivity { get; }

        /// <summary>
        ///     Log message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        ///     Exception that triggered this log.
        /// </summary>
        public Exception? Exception { get; }

        public LogEntry(LogSensitivity sensitivity, string message, Exception? exception = null)
        {
            Sensitivity = sensitivity;
            Message = message;
            Exception = exception;
        }
    }
}
