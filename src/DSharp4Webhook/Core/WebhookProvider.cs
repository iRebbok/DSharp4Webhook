using DSharp4Webhook.Internal;
using DSharp4Webhook.Rest;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     Manager of all created Webhooks.
    /// </summary>
    public sealed class WebhookProvider : IDisposable
    {
        #region Static Properties

        /// <summary>
        ///     Regular expression for parsing the webhook Url.
        /// </summary>
        public static readonly Regex WebhookUrlRegex = new Regex(@"^.*discord(?:app)?\.com\/api\/webhooks\/([\d]+)\/([a-z0-9_-]+)$",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        /// <summary>
        ///     Base url for generating a webhook url if it was created using a token and id.
        /// </summary>
        public static readonly string WebhookBaseUrl = "https://discord.com/api/webhooks/{0}/{1}";

        /// <summary>
        ///     Base url for generating avatar urls.
        /// </summary>
        public static readonly string WebhookBaseAvatarUrl = "https://cdn.discordapp.com/avatars/{0}/{1}.{2}";

        /// <summary>
        ///     The maximum number of characters that can be sent as a message.
        /// </summary>
        public static readonly int MAX_CONTENT_LENGTH = 2000;
        /// <summary>
        ///     Minimum limit on the number of characters in a nickname.
        /// </summary>
        public static readonly int MIN_NICKNAME_LENGTH = 1;
        /// <summary>
        ///     Maximum limit on the number of characters in a nickname.
        /// </summary>
        public static readonly int MAX_NICKNAME_LENGTH = 80;
        /// <summary>
        ///     Maximum limit on attachments in a message.
        /// </summary>
        public static readonly int MAX_ATTACHMENTS = 10;
        /// <summary>
        ///     Limit on the size of all attachments.
        /// </summary>
        public static readonly int MAX_ATTACHMENTS_SIZE = 8 * 1024 * 1024; // 8 MB currently
        /// <summary>
        ///     Maximum character limit in embed title.
        /// </summary>
        public static readonly int MAX_EMBED_TITLE_LENGTH = 256;
        /// <summary>
        ///     Maximum character limit in embed description.
        /// </summary>
        public static readonly int MAX_EMBED_DESCRIPTION_LENGTH = 2048;
        /// <summary>
        ///     Maximum limit on the number of footers in embed.
        /// </summary>
        public static readonly int MAX_EMBED_FIELDS_COUNT = 25;
        /// <summary>
        ///     Maximum character limit of name in embed field.
        /// </summary>
        public static readonly int MAX_EMBED_FIELD_NAME_LENGTH = 256;
        /// <summary>
        ///     Maximum character limit of value in embed field.
        /// </summary>
        public static readonly int MAX_EMBED_FIELD_VALUE_LENGTH = 1024;
        /// <summary>
        ///     Maximum character limit of text in embed footer.
        /// </summary>
        public static readonly int MAX_EMBED_FOOTER_TEXT_LENGTH = 2048;
        /// <summary>
        ///     Maximum character limit of name in embed author.
        /// </summary>
        public static readonly int MAX_EMBED_AUTHOR_NAME_LENGTH = 256;
        /// <summary>
        ///     The maximum number of characters in all such as
        ///     title, description, field.name, field.value,
        ///     footer.text and author.name.
        /// </summary>
        public static readonly int MAX_EMBED_LENGTH = 6000;

        #endregion

        #region Properties

        /// <summary>
        ///     Unique identifier.
        /// </summary>
        public string Id { get; }

        private RestSettings _restSettings;
        /// <summary>
        ///     Rest settings which will use the following webhook when creating it.
        /// </summary>
        public RestSettings RestSettings { get => _restSettings; set => _restSettings = value ?? _restSettings; }

        /// <summary>
        ///     Allowed mentions to use when creating webhooks.
        /// </summary>
        public AllowedMention AllowedMention { get; set; }

        #endregion

        #region Fields

        /// <summary>
        ///     Stores all registered webhooks as Id-Webhook.
        /// </summary>
        private readonly Dictionary<ulong, IWebhook> _webhooks;

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
            _restSettings = new RestSettings();
        }

        #region Static Methods

        /// <summary>
        ///     Sets the rest of the provider to be used in the application.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Type is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Provider not suitable.
        /// </exception>
        /// <remarks>
        ///     Since dotnet just doesn't load all the assemblies
        ///     that are inserted in the project if they are not used,
        ///     I have to use this...
        /// </remarks>
        public static void SetRestProvider(Type type)
        {
            RestProviderLoader.SetProviderType(type);
        }

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
        public static IWebhook CreateStaticWebhook(string url)
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
            if (!match.Success) throw new InvalidOperationException("The url is not valid");

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
        ///     Removes the webhook from the collection.
        /// </summary>
        /// <param name="id">
        ///     Webhook id.
        /// </param>
        /// <returns>
        ///     true if the webhook was found in the collection and deleted,
        ///     otherwise false.
        /// </returns>
        public bool RemoveWebhookById(ulong id)
        {
            return _webhooks.Remove(id);
        }

        /// <summary>
        ///     Removes the webhook from the collection.
        /// </summary>
        /// <param name="webhook">
        ///     Webhook instance.
        /// </param>
        /// <returns>
        ///     true if the webhook was found in the collection and deleted,
        ///     otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     If webhook instance is null.
        /// </exception>
        public bool RemoveWebhook(IWebhook webhook)
        {
            Checks.CheckForNull(webhook, nameof(webhook), "Webhook cannot be null to delete");
            return _webhooks.Remove(webhook.Id);
        }

        /// <summary>
        ///     Tries to remove a webhook from the collection.
        /// </summary>
        /// <param name="webhook">
        ///     Webhook instance.
        /// </param>
        ///     true if the webhook was found in the collection and deleted,
        ///     otherwise false.
        public bool TryRemoveWebhook(IWebhook webhook)
        {
            return webhook != null && _webhooks.Remove(webhook.Id);
        }

        /// <summary>
        ///     Returns all webhooks that are contained in the collection.
        /// </summary>
        /// <returns>
        ///     Webhooks instances.
        /// </returns>
        public IWebhook[] GetWebhooks()
        {
            return _webhooks.Select(x => x.Value).ToArray();
        }

        public void Dispose()
        {
            // Just take out each webhook and call Dispose from it
            foreach (var webhook in _webhooks.Values)
                webhook.Dispose();
            _webhooks.Clear();
        }

        #endregion
    }
}
