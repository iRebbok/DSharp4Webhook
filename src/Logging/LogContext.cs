using DSharp4Webhook.Core;

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
        ///     Webhook that called this log.
        /// </summary>
        public IWebhook Webhook { get; }

        /// <summary>
        ///     Determines whether the sender is a webhook.
        /// </summary>
        public bool FromWebhook { get => Webhook != null ? true : _fromWebhook != null ? (bool)_fromWebhook : false; }
        private bool? _fromWebhook;

        public LogContext(LogSensitivity sensitivity, string message, IWebhook webhook, bool? fromWebhook = null)
        {
            Sensitivity = sensitivity;
            Message = message;
            Webhook = webhook;
            _fromWebhook = fromWebhook;
        }
    }
}
