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
        ///     Determines whether the sender is a webhook.
        /// </summary>
        public bool FromWebhook { get; }

        public LogContext(LogSensitivity sensitivity, string message, bool fromWebhook = true)
        {
            Sensitivity = sensitivity;
            Message = message;
            FromWebhook = fromWebhook;
        }
    }
}
