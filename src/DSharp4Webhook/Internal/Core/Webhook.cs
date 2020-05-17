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
        public ulong Id { get => _id; }
        public string Token { get => _token; }

        private readonly WebhookProvider _provider;
        private readonly BaseRestProvider _restProvider;
        private readonly ActionManager _actionManager;

        private WebhookStatus _status;

        private readonly ulong _id;
        private readonly string _token;
        private readonly string _url;

        /// <summary>
        ///     Creates a webhook.
        /// </summary>
        /// <param name="provider">
        ///     Provider of the webhook, may be null.
        /// </param>
        /// <param name="id">
        ///     Webhook id.
        /// </param>
        /// <param name="token">
        ///     Webhook token.
        /// </param>
        /// <param name="url">
        ///     Webhook url.
        /// </param>
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

            _restProvider = RestProviderLoader.CreateProvider(this);
            _provider = provider;
            _actionManager = new ActionManager(this);
            // Setting the unverified status
            _status = WebhookStatus.NOT_CHECKED;
        }

        public void Dispose()
        {
            _actionManager.Dispose();
        }

        public string GetWebhookUrl()
        {
            return _url;
        }

        public IMessageAction SendMessage(string message, bool isTTS = false)
        {
            return new MessageAction(new Message(message, isTTS), this);
        }

        public IMessageAction SendMessage(IMessage message)
        {
            return new MessageAction(message, this);
        }

        public IInfoAction GetInfo()
        {
            return new InfoAction(this);
        }

        public IDeleteAction Delete()
        {
            return new DeleteAction(this);
        }

        public IModifyAction Modify(string name)
        {
            if (name != null)
            {
                name = name.Trim();
                if (name.Length <= WebhookProvider.MIN_NICKNAME_LENGHT || name.Length >= WebhookProvider.MIN_NICKNAME_LENGHT)
                    throw new ArgumentOutOfRangeException(nameof(name), $"Must be between {WebhookProvider.MIN_NICKNAME_LENGHT} and {WebhookProvider.MAX_NICKNAME_LENGTH} in length.");
            }

            var data = new ModifyContent();
            data.name = name;
            return new ModifyAction(data.Serialize(), this);
        }

        public IModifyAction Modify(string name, IWebhookImage image)
        {
            if (name != null)
            {
                name = name.Trim();
                if (name.Length <= WebhookProvider.MIN_NICKNAME_LENGHT || name.Length >= WebhookProvider.MIN_NICKNAME_LENGHT)
                    throw new ArgumentOutOfRangeException(nameof(name), $"Must be between {WebhookProvider.MIN_NICKNAME_LENGHT} and {WebhookProvider.MAX_NICKNAME_LENGTH} in length.");
            }

            var data = new ModifyContent();
            data.name = name;
            data.avatar = image == null ? null : image.ToUriScheme();
            return new ModifyAction(data.Serialize(), this);
        }
    }
}
