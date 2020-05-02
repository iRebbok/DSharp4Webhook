using DSharp4Webhook.Internal;
using DSharp4Webhook.Logging;
using System;
using System.Text.RegularExpressions;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     Provider for creating a Webhook.
    /// </summary>
    public sealed class WebhookProvider : ILogable
    {
        // Prohibiting creation, it should give the impression of a static class.
        private WebhookProvider() { }

        /// <summary>
        ///     Static object reference.
        /// </summary>
        public static WebhookProvider Provider { get; } = new WebhookProvider();

        public static Regex WebhookUrlRegex { get; } = new Regex(@"^.*discordapp\.com\/api\/webhooks\/([\d]+)\/([a-z0-9_-]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public IWebhook CreateWebhook(string url)
        {
            Match match = WebhookUrlRegex.Match(url);
            return new WebhookImpl(ulong.Parse(match.Groups[1].Value), match.Groups[2].Value, url);
        }

        public IWebhook CreateWebhook(ulong id, string token)
        {
            return new WebhookImpl(id, token, $"https://discordapp.com/api/webhooks/{id}/{token}");
        }

        /// <summary>
        ///     Collects all logs that are called during operation.
        /// </summary>
        public event Action<LogContext> OnLog;

        public void Log(LogContext context)
        {
            OnLog?.Invoke(context);
        }
    }
}
