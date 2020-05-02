using System;

namespace DSharp4Webhook.Logging
{
    public interface ILogable
    {
        /// <summary>
        ///     Invoked when displaying the logging.
        /// </summary>
        event Action<LogContext> OnLog;

        /// <summary>
        ///     Calls the <see cref="OnLog"/> event.
        /// </summary>
        void Log(LogContext context);
    }
}
