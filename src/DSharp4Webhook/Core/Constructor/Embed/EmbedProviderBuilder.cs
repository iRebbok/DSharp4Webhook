using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Internal.Embed;

namespace DSharp4Webhook.Core.Constructor
{
    public sealed class EmbedProviderBuilder : IBuilder
    {
#nullable enable
        private string? _name;
        private string? _url;

        public string? Name
        {
            get => _name;
            set => _name = value;
        }

        public string? Url
        {
            get => _url;
            set => _url = value;
        }
#nullable restore

        public static EmbedProviderBuilder New() => new EmbedProviderBuilder();

        private EmbedProviderBuilder() { }

        public IEmbedProvider Build()
        {
            return new EmbedProvider(this);
        }

        public void Reset()
        {
            _name = null;
            _url = null;
        }
    }
}
