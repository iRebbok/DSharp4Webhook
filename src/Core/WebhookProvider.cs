using DSharp4Webhook.Logging;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     Manager of all created Webhooks.
    /// </summary>
    public class WebhookProvider
    {
        #region Properties

        /// <summary>
        ///     Regular expression for parsing the webhook Url.
        /// </summary>
        public static Regex WebhookUrlRegex { get; } = new Regex(@"^.*discordapp\.com\/api\/webhooks\/([\d]+)\/([a-z0-9_-]+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        ///     Base url for generating a webhook url if it was created using a token and id.
        /// </summary>
        public static string WebhookBaseUrl { get; } = "https://discordapp.com/api/webhooks/{0}/{1}";

        /// <summary>
        ///     The maximum number of characters that can be sent as a message.
        /// </summary>
        public static int MAX_CONTENT_LENGTH { get; } = 2000;
        /// <summary>
        ///     Minimum limit on the number of characters in a nickname.
        /// </summary>
        public static int MIX_NICKNAME_LENGHT { get; } = 1;
        /// <summary>
        ///     Maximum limit on the number of characters in a nickname.
        /// </summary>
        public static int MAX_NICKNAME_LENGTH { get; } = 80;

        /// <summary>
        ///     Unique identifier.
        /// </summary>
        public string Id { get; }

        #endregion

        #region Fields

        /// <summary>
        ///     Stores all registered webhooks as Id-Webhook.
        /// </summary>
        private readonly Dictionary<ulong, IWebhook> _webhooks;

        /// <summary>
        ///     It is the main event provider for the provider.
        /// </summary>
        public event Action<LogContext> OnLog;

        #endregion

        /// <summary>
        ///     Creates an instance of the provider.
        /// </summary>
        /// <param name="id">
        ///     Unique identifier.
        /// </param>
        public WebhookProvider(string id)
        {
            Id = !string.IsNullOrEmpty(id) ? id : throw new ArgumentException("Unique identifier cannot be null or empty", nameof(id));

            _webhooks = new Dictionary<ulong, IWebhook>();
        }

        #region Static Methods

        /// <summary>
        ///     Creates a webhack with url without binding to the provider,
        ///     these webhooks do not have logging.
        /// </summary>
        /// <param name="url">
        ///     Webhook url.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If the url is empty or null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If the url has an invalid format.
        /// </exception>
        public static IWebhook CreateSaticWebhook(string url)
        {
            return CreateWebhook(url, null);
        }

        /// <summary>
        ///     Creates a webhook instane without binding to the provider,
        ///     these webhooks do not have logging.
        /// </summary>
        /// <param name="id">
        ///     Webhook id.
        /// </param>
        /// <param name="token">
        ///     Token webjuice for authorization.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If the token is empty or null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If the webhook already exists.
        /// </exception>
        public static IWebhook CreateStaticWebhook(ulong id, string token)
        {
            return CreateWebhook(id, token, null);
        }

        /// <remarks>
        ///     Wrapper for creating webhooks.
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///     If the url is empty or null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If the url has an invalid format.
        /// </exception>
        private static IWebhook CreateWebhook(string url, WebhookProvider provider)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentException("Url cannot be null or empty", nameof(url));

            Match match = WebhookUrlRegex.Match(url);
            if (!match.Success) throw new InvalidOperationException("");

            Webhook webhook = new Webhook(provider, ulong.Parse(match.Groups[1].Value), match.Groups[2].Value, url);
            provider?._webhooks.Add(webhook.Id, webhook);

            return webhook;
        }

        /// <remarks>
        ///     Wrapper for creating webhooks.
        /// </remarks>
        /// <exception cref="ArgumentException">
        ///     If the url is empty or null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If the url has an invalid format or the webhook already exists.
        /// </exception>
        private static IWebhook CreateWebhook(ulong id, string token, WebhookProvider provider)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentException("Token cannot be null or empty", nameof(token));
            if (provider?._webhooks.ContainsKey(id) ?? false) throw new InvalidOperationException($"Webhook id {id} is already in the collection");

            Webhook webhook = new Webhook(provider, id, token, string.Format(WebhookBaseUrl, id, token));
            provider?._webhooks.Add(webhook.Id, webhook);

            return webhook;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Creates a new Webhook from a url and uses its url.
        /// </summary>
        /// <param name="url">
        ///     Webhook url.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If the url is empty or null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If the url has an invalid format or the webhook already exists.
        /// </exception>
        public IWebhook CreateWebhook(string url)
        {
            return CreateWebhook(url, this);
        }

        /// <summary>
        ///     Creates a webhook instane.
        /// </summary>
        /// <param name="id">
        ///     Webhook id.
        /// </param>
        /// <param name="token">
        ///     Token webjuice for authorization.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If the token is empty or null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If the webhook already exists.
        /// </exception>
        public IWebhook CreateWebhook(ulong id, string token)
        {
            return CreateWebhook(id, token, this);
        }

        /// <summary>
        ///     Gets the webhook by id.
        /// </summary>
        /// <param name="id">
        ///     Webhook id.
        /// </param>
        /// <exception cref="KeyNotFoundException">
        ///     If the webhook doesn't exist.
        /// </exception>
        public IWebhook GetWebhookById(ulong id)
        {
            return _webhooks[id];
        }

        /// <summary>
        ///     Tries to get the webhook by id.
        /// </summary>
        /// <param name="id">
        ///     Webhook id.
        /// </param>
        /// <param name="webhook">
        ///     Webhook instance.
        ///     null if the result is false.
        /// </param>
        /// <returns>
        ///     true if the collection contains an element with
        ///     the specified key; otherwise, false.
        /// </returns>
        public bool TryGetWebhookById(ulong id, out IWebhook webhook)
        {
            return _webhooks.TryGetValue(id, out webhook);
        }

        /// <summary>
        ///     Adds the specified webhook to the collection,
        ///     useful when implementing a custom webhook.
        /// </summary>
        /// <param name="webhook">
        ///     Webhook instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     If the webhook is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If the webhook is already in the collection.
        /// </exception>
        public void AddWebhook(IWebhook webhook)
        {
            if (webhook == null) throw new ArgumentNullException(nameof(webhook), "The webhook can't be null");
            if (_webhooks.ContainsKey(webhook.Id)) throw new InvalidOperationException("The webhook is already contained in the collection");

            _webhooks.Add(webhook.Id, webhook);
        }

        /// <summary>
        ///     Tries to add a webhook to the collection.
        /// </summary>
        /// <param name="webhook">
        ///     Webhook instance.
        /// </param>
        /// <returns>
        ///     true if the webhook was added to the collection; otherwise, false
        /// </returns>
        public bool TryAddWebhook(IWebhook webhook)
        {
            bool allow = webhook != null && !_webhooks.ContainsKey(webhook.Id);
            if (allow)
                _webhooks.Add(webhook.Id, webhook);

            return allow;
        }

        /// <summary>
        ///     Sends logs to all subscribed channels.
        /// </summary>
        /// <remarks>
        ///     The user does not need to send logs.
        /// </remarks>
        internal void Log(LogContext context)
        {
            OnLog?.Invoke(context);
        }

        #endregion
    }
}
