using DSharp4Webhook.Core;
using DSharp4Webhook.Util;
using System;
using System.IO;

namespace DSharp4Webhook.Internal
{
    internal sealed class WebhookImage : IWebhookImage
    {
        public byte[] Data { get => _data; }

        private byte[] _data;
        private string _uriCached;

        public WebhookImage(byte[] data)
        {
            Checks.CheckForNull(data, nameof(data));
            _data = data;
        }

        public WebhookImage(FileInfo file)
        {
            Checks.CheckForNull(file, nameof(file));
            Checks.CheckForArgument(!file.Exists, nameof(file));
            using (var stream = file.OpenRead())
            {
                _data = new byte[stream.Length];
                stream.Read(_data, 0, _data.Length);
            }

        }

        public WebhookImage(Stream stream)
        {
            Checks.CheckForNull(stream, nameof(stream));
            Checks.CheckForArgument(!stream.CanSeek || !stream.CanRead, nameof(stream));

            _data = new byte[stream.Length - stream.Position];
            stream.Read(_data, 0, _data.Length);
        }

        public void Save(string path)
        {
            Checks.CheckForArgument(string.IsNullOrEmpty(path), nameof(path));
            File.WriteAllBytes(path, _data);
        }

        public string ToUriScheme()
        {
            if (!string.IsNullOrEmpty(_uriCached))
                return _uriCached;

            return _uriCached = $"data:image/png;base64,{Convert.ToBase64String(_data)}";
        }
    }
}
