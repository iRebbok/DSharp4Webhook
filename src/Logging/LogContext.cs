using System;

namespace DSharp4Webhook.Logging
{
    /// <remarks>
    ///     It is the main object for delivering logs to the client.
    /// </remarks>
    public struct LogContext
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
        ///     Id of the webhack that triggered this logging.
        /// </summary>
        public ulong WebhookId { get; }

        /// <summary>
        ///     Exception that triggered this log.
        /// </summary>
        public Exception Exception { get; }

        public LogContext(LogSensitivity sensitivity, string message, ulong? webhookId = null, Exception exception = null)
        {
            Sensitivity = sensitivity;
            Message = message;
            WebhookId = webhookId ?? 0;
            Exception = exception;
        }
    }
}
