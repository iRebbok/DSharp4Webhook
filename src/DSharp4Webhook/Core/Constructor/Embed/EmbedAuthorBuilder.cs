using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Internal.Embed;
using DSharp4Webhook.Util;
using System;

namespace DSharp4Webhook.Core.Constructor
{
    public sealed class EmbedAuthorBuilder : IBuilder
    {
        private string? _name;
        private string? _url;
        private string? _iconUrl;
        private string? _proxyIconUrl;

        #region Properties

        /// <exception cref="ArgumentOutOfRangeException">
        ///     Exceeds the allowed length.
        /// </exception>
        public string? Name
        {
            get => _name;
            set
            {
                if (!(value is null))
                {
                    value = value.Trim();
                    Contract.CheckBounds(nameof(Name), $"Must be no more than {WebhookProvider.MAX_EMBED_AUTHOR_NAME_LENGTH} in length",
                        WebhookProvider.MAX_EMBED_AUTHOR_NAME_LENGTH + 1, value.Length);
                    _name = value;
                }
                else
                    _name = value;
            }
        }

        public string? Url
        {
            get => _url;
            set => _url = value;
        }

        public string? IconUrl
        {
            get => _iconUrl;
            set => _iconUrl = value;
        }

        public string? ProxyIconUrl
        {
            get => _proxyIconUrl;
            set => _proxyIconUrl = value;
        }

        #endregion

        public static EmbedAuthorBuilder New() => new EmbedAuthorBuilder();

        private EmbedAuthorBuilder() { }

        public IEmbedAuthor Build()
        {
            return new EmbedAuthor(this);
        }

        public void Reset()
        {
            _name = null;
            _url = null;
            _iconUrl = null;
            _proxyIconUrl = null;
        }
    }
}
