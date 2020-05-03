using DSharp4Webhook.Core;

namespace DSharp4Webhook.Logging
{
    /// <summary>
    ///     Simple provider for log calls.
    /// </summary>
    public static class LogProvider
    {
        public static void Log(LogContext context)
        {
            if (context.Webhook != null) context.Webhook.Log(context);
            // Freeing global logs from unnecessary data
            WebhookProvider.Provider.Log(new LogContext(context.Sensitivity, context.Message, null));
        }
    }
}
