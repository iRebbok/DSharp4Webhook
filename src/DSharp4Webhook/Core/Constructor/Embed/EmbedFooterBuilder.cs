using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Internal.Embed;
using DSharp4Webhook.Util;
using System;

namespace DSharp4Webhook.Core.Constructor
{
    public sealed class EmbedFooterBuilder : IBuilder
    {
        private string? _text;
        private string? _iconUrl;
        private string? _proxyIconUrl;

        #region Properties

        /// <exception cref="ArgumentNullException">
        ///     Attempt to assign a null value.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Exceeds the allowed length.
        /// </exception>
        public string? Text
        {
            get => _text;
            set
            {
                if (value is null)
                    throw new ArgumentNullException("Value cannot be null", nameof(Text));

                value = value.Trim();
                Contract.CheckBounds(nameof(Text), $"Must be no more than {WebhookProvider.MAX_EMBED_FOOTER_TEXT_LENGTH} in length",
                    WebhookProvider.MAX_EMBED_FOOTER_TEXT_LENGTH + 1, value.Length);
                _text = value;
            }
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

        public static EmbedFooterBuilder New() => new EmbedFooterBuilder();

        private EmbedFooterBuilder() { }

        /// <exception cref="InvalidOperationException">
        ///     Attempt to build without set text.
        /// </exception>
        public IEmbedFooter Build()
        {
            if (_text is null)
                throw new InvalidOperationException("Text cannot be null");

            return new EmbedFooter(this);
        }

        public void Reset()
        {
            _text = null;
            _iconUrl = null;
            _proxyIconUrl = null;
        }
    }
}
