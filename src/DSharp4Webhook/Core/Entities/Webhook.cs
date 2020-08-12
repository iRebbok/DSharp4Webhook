using DSharp4Webhook.Actions;
using DSharp4Webhook.Actions.Rest;
using DSharp4Webhook.Core;
using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Rest;
using DSharp4Webhook.Rest.Manipulation;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSharp4Webhook.Internal
{
    public class Webhook : IWebhook
    {
        #region Properties

        public WebhookProvider? Provider { get; }
        public WebhookStatus Status
        {
            get => _status;
            set
            {
                // Don't allow to change the status for nonexistent webhooks
                if (_status == WebhookStatus.NOT_EXIST)
                    throw new InvalidOperationException("Attempt to assign a third-party status to a nonexistent webhook");
                // Don't allow to downgrade the status for existing webhooks
                if (_status == WebhookStatus.EXIST && value == WebhookStatus.NOT_CHECKED)
                    throw new InvalidOperationException("Attempt to downgrade the status of an existing web hook");

                _status = value;
            }
        }

        public RestSettings RestSettings { get; set; }
        public AllowedMention AllowedMention { get; set; }
        public ulong Id { get; }
        public string Token { get; }
        public string WebhookUrl { get; }

        public BaseRestProvider RestProvider { get; }
        public IActionManager ActionManager { get; }

        #endregion

        #region Fields

        private WebhookStatus _status;

        #endregion

        /// <summary>
        ///     Creates new instanse of <see cref="Webhook"/>.
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     If the url or token is null or empty.
        /// </exception>
        public Webhook(WebhookProvider? provider, ulong id, string token, string url)
        {
            Contract.AssertArgumentNotTrue(string.IsNullOrEmpty(token), nameof(token), "The token can't be empty or null");
            Contract.AssertArgumentNotTrue(string.IsNullOrEmpty(url), nameof(url), "The url can't be empty or null");

            Id = id;
            Token = token;
            WebhookUrl = url;

            // Setting the unverified status
            _status = WebhookStatus.NOT_CHECKED;
            AllowedMention = provider?.AllowedMention ?? AllowedMention.NONE;
            RestSettings = provider?.RestSettings ?? new RestSettings();

            RestProvider = RestProviderLoader.CreateProvider(this);
            Provider = provider;
            ActionManager = new ActionManager(this);
        }

        #region Methods

        ~Webhook() => Dispose(true);

        public void Dispose() => Dispose(false);

        private void Dispose(bool disposing)
        {
            ActionManager.Dispose();
            RestProvider.Dispose();
            if (!disposing)
                GC.SuppressFinalize(this);
        }

        public IMessageAction SendMessage(string message, bool isTTS = false, IMessageMention? messageMention = null, RestSettings? restSettings = null)
        {
            Contract.AssertNotNull(message, nameof(message));

            message = message.Trim();
            Contract.CheckBounds(nameof(message), $"The text cannot exceed the {WebhookProvider.MAX_CONTENT_LENGTH} character limit",
                WebhookProvider.MAX_CONTENT_LENGTH, message.Length);

            messageMention ??= new MessageMention(AllowedMention);
            restSettings ??= RestSettings;

            return new MessageAction(new Message(message, messageMention, isTTS), this, restSettings.Value);
        }

        public IMessageAction SendMessage(IMessage message, RestSettings? restSettings = null)
        {
            return new MessageAction(message, this, restSettings ?? RestSettings);
        }

        public IMessageAction SendMessage(IEnumerable<IEmbed> embeds, IMessageMention? messageMention = null, RestSettings? restSettings = null)
        {
            Contract.AssertNotNull(embeds, nameof(embeds));
            var embedCount = embeds.Count();
            Contract.AssertArgumentNotTrue(embedCount == 0, nameof(embeds));
            Contract.CheckBounds(nameof(embeds), null, WebhookProvider.MAX_EMBED_COUNT + 1, embedCount);

            messageMention ??= new MessageMention(AllowedMention);
            restSettings ??= RestSettings;

            return new MessageAction(new Message(embeds, messageMention), this, RestSettings);
        }

        public IMessageAction SendMessage(IEmbed embed, IMessageMention? messageMention = null, RestSettings? restSettings = null)
        {
            Contract.AssertNotNull(embed, nameof(embed));
            // Just passing it on
            return SendMessage(new[] { embed }, messageMention, restSettings);
        }

        public IInfoAction GetInfo(RestSettings? restSettings = null)
        {
            return new InfoAction(this, restSettings ?? RestSettings);
        }

        public IDeleteAction Delete(RestSettings? restSettings = null)
        {
            return new DeleteAction(this, restSettings ?? RestSettings);
        }

        public IModifyAction Modify(string name, RestSettings? restSettings = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            // todo: do not hardcode
            else if (name.Equals("clyde", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Webhook name cannot be 'clyde'", nameof(name));

            name = name.Trim();
            Contract.CheckBounds(nameof(name), $"Must be between {WebhookProvider.MIN_NICKNAME_LENGTH} and {WebhookProvider.MAX_NICKNAME_LENGTH} in length.", name.Length,
                WebhookProvider.MAX_NICKNAME_LENGTH + 1);
            Contract.CheckBoundsUnderside(nameof(name), $"Must be between {WebhookProvider.MIN_NICKNAME_LENGTH} and {WebhookProvider.MAX_NICKNAME_LENGTH} in length.", name.Length,
                WebhookProvider.MAX_NICKNAME_LENGTH + 1);

            var data = new ModifyContent(name, WebhookImage.Empty, false);
            return new ModifyAction(data.Serialize(), this, restSettings ?? RestSettings);
        }

        public IModifyAction Modify(string name, IWebhookImage? image, RestSettings? restSettings = null)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            // todo: do not hardcode
            else if (name.Equals("clyde", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Webhook name cannot be 'clyde'", nameof(name));

            name = name.Trim();
            if (name.Length <= WebhookProvider.MIN_NICKNAME_LENGTH || name.Length >= WebhookProvider.MIN_NICKNAME_LENGTH)
                throw new ArgumentOutOfRangeException(nameof(name), $"Must be between {WebhookProvider.MIN_NICKNAME_LENGTH} and {WebhookProvider.MAX_NICKNAME_LENGTH} in length.");

            var data = new ModifyContent(name, image, false);
            return new ModifyAction(data.Serialize(), this, restSettings ?? RestSettings);
        }

        public IModifyAction Modify(IModifyContent content, RestSettings? restSettings = null)
        {
            Contract.AssertNotNull(content, nameof(content));
            return new ModifyAction(content.Serialize(), this, restSettings ?? RestSettings);
        }

        #endregion
    }
}
