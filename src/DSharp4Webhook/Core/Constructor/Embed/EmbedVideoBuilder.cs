using DSharp4Webhook.Core.Embed;
using DSharp4Webhook.Internal.Embed;
using System;

namespace DSharp4Webhook.Core.Constructor
{
    public sealed class EmbedVideoBuilder : IBuilder
    {
        private uint? _height;
        private uint? _width;
        private string? _url;

        #region Properties

        public string? Url
        {
            get => _url;
            set => _url = value;
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

        public static EmbedVideoBuilder New() => new EmbedVideoBuilder();

        private EmbedVideoBuilder() { }

        public IEmbedVideo Build()
        {
            return new EmbedVideo(this);
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
