using DSharp4Webhook.Action;
using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest;
using DSharp4Webhook.Rest.Manipulation;
using DSharp4Webhook.Util;
using System;

namespace DSharp4Webhook.Internal
{
    internal sealed class Webhook : IWebhook
    {
        public BaseRestProvider RestProvider { get => _restProvider; }
        public IActionManager ActionManager { get => _actionManager; }
        public WebhookProvider Provider { get => _provider; }
        public WebhookStatus Status
        {
            get => _status;
            set
            {
                // Don't allow to change the status for nonexistent webhooks
                if (_status == WebhookStatus.NOT_EXISTING)
                    throw new InvalidOperationException("Attempt to assign a third-party status to a nonexistent webhook");
                // Don't allow to downgrade the status for existing webhooks
                if (_status == WebhookStatus.EXISTING && value == WebhookStatus.NOT_CHECKED)
                    throw new InvalidOperationException("Attempt to downgrade the status of an existing web hook");

                _status = value;
            }
        }
        public RestSettings RestSettings
        {
            get => _restSettings;
            set => _restSettings = value ?? _restSettings;
        }
        public AllowedMention AllowedMention
        {
            get => _allowedMention;
            set => _allowedMention = value;
        }
        public ulong Id { get => _id; }
        public string Token { get => _token; }

        private readonly WebhookProvider _provider;
        private readonly BaseRestProvider _restProvider;
        private readonly ActionManager _actionManager;

        private RestSettings _restSettings;
        private WebhookStatus _status;
        private AllowedMention _allowedMention;

        private readonly ulong _id;
        private readonly string _token;
        private readonly string _url;

        /// <exception cref="ArgumentException">
        ///     If the url or token is null or empty.
        /// </exception>
        public Webhook(WebhookProvider provider, ulong id, string token, string url)
        {
            Checks.CheckForArgument(string.IsNullOrEmpty(token), nameof(token), "The token can't be empty or null");
            Checks.CheckForArgument(string.IsNullOrEmpty(url), nameof(url), "The url can't be empty or null");

            _id = id;
            _token = token;
            _url = url;

            // Setting the unverified status
            _status = WebhookStatus.NOT_CHECKED;
            _allowedMention = provider?.AllowedMention ?? AllowedMention.NONE;

            _restProvider = RestProviderLoader.CreateProvider(this);
            _provider = provider;
            _actionManager = new ActionManager(this);
            _restSettings = provider?.RestSettings ?? new RestSettings();
        }

        public void Dispose()
        {
            _actionManager.Dispose();
        }

        public string GetWebhookUrl()
        {
            return _url;
        }

        public IMessageAction SendMessage(string message, bool isTTS = false, IMessageMention messageMention = null, RestSettings restSettings = null)
        {
            messageMention = messageMention ?? new MessageMention(_allowedMention);
            restSettings = restSettings ?? _restSettings;

            return new MessageAction(new Message(message, messageMention, isTTS), this, restSettings);
        }

        public IMessageAction SendMessage(IMessage message, RestSettings restSettings = null)
        {
            return new MessageAction(message, this, restSettings ?? _restSettings);
        }

        public IInfoAction GetInfo(RestSettings restSettings = null)
        {
            return new InfoAction(this, restSettings ?? _restSettings);
        }

        public IDeleteAction Delete(RestSettings restSettings = null)
        {
            return new DeleteAction(this, restSettings ?? _restSettings);
        }

        public IModifyAction Modify(string name, RestSettings restSettings = null)
        {
            if (name != null)
            {
                name = name.Trim();
                if (name.Length <= WebhookProvider.MIN_NICKNAME_LENGTH || name.Length >= WebhookProvider.MIN_NICKNAME_LENGTH)
                    throw new ArgumentOutOfRangeException(nameof(name), $"Must be between {WebhookProvider.MIN_NICKNAME_LENGTH} and {WebhookProvider.MAX_NICKNAME_LENGTH} in length.");
            }

            var data = new ModifyContent();
            data.name = name;
            return new ModifyAction(data.Serialize(), this, restSettings ?? _restSettings);
        }

        public IModifyAction Modify(string name, IWebhookImage image, RestSettings restSettings = null)
        {
            if (name != null)
            {
                name = name.Trim();
                if (name.Length <= WebhookProvider.MIN_NICKNAME_LENGTH || name.Length >= WebhookProvider.MIN_NICKNAME_LENGTH)
                    throw new ArgumentOutOfRangeException(nameof(name), $"Must be between {WebhookProvider.MIN_NICKNAME_LENGTH} and {WebhookProvider.MAX_NICKNAME_LENGTH} in length.");
            }

            var data = new ModifyContent();
            data.name = name;
            data.avatar = image == null ? null : image.ToUriScheme();
            return new ModifyAction(data.Serialize(), this, restSettings ?? _restSettings);
        }
    }
}
