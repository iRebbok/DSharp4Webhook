using DSharp4Webhook.Core;
using System;

namespace DSharp4Webhook.Logging
{
    /// <summary>
    ///     Simple provider for log calls.
    /// </summary>
    public static class LogProvider
    {
        public static void Log(IWebhook webhook, LogContext context)
        {
            (webhook ?? throw new ArgumentNullException(nameof(webhook))).Log(context);
            WebhookProvider.Provider.Log(context);
        }
    }
}
