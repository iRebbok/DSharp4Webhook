namespace DSharp4Webhook.Logging
{
    /// <remarks>
    ///     It is the main object for delivering logs to the client.
    /// </remarks>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public readonly struct LogContext
#pragma warning restore CA1815 // Override equals and operator equals on value types
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
        public System.Exception? Exception { get; }

        public LogContext(LogSensitivity sensitivity, string message, ulong? webhookId = null, System.Exception? exception = null)
        {
            Sensitivity = sensitivity;
            Message = message;
            WebhookId = webhookId ?? 0;
            Exception = exception;
        }
    }
}
