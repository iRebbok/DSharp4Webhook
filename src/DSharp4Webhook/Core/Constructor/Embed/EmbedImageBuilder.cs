using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Internal.Embed;

namespace DSharp4Webhook.Core.Constructor
{
    public sealed class EmbedImageBuilder : IBuilder
    {
        private uint? _height;
        private uint? _width;
        private string? _url;
        private string? _proxyUrl;

        #region Properties

        public string? Url
        {
            get => _url;
            set => _url = value;
        }

        public string? ProxyUrl
        {
            get => _proxyUrl;
            set => _proxyUrl = value;
        }

        public uint? Height
        {
            get => _height;
            set => _height = value;
        }

        public uint? Width
        {
            get => _width;
            set => _width = value;
        }

        #endregion

        public static EmbedImageBuilder New() => new EmbedImageBuilder();

        private EmbedImageBuilder() { }

        public IEmbedImage Build()
        {
            return new EmbedImage(this);
        }

        public void Reset()
        {
            _height = null;
            _width = null;
            _url = null;
            _proxyUrl = null;
        }
    }
}
