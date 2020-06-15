using DSharp4Webhook.Core;
using DSharp4Webhook.Util;
using DSharp4Webhook.Util.Extensions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DSharp4Webhook.Internal
{
    internal sealed class WebhookImage : IWebhookImage
    {
        /// <summary>
        ///     An empty image that will not modify the webhook.
        /// </summary>
        public static WebhookImage Empty { get; } = new WebhookImage();

        public ReadOnlyCollection<byte>? Data { get => _data; }

        private ReadOnlyCollection<byte>? _data;
        private string? _uriCached;

        private WebhookImage() { }

        public WebhookImage(byte[] data)
        {
            Checks.CheckForNull(data, nameof(data));
            _data = data.ToReadOnlyCollection()!;
        }

        public WebhookImage(FileInfo file)
        {
            Checks.CheckForNull(file, nameof(file));
            Checks.CheckForArgument(!file.Exists, nameof(file));
            using var stream = file.OpenRead();
            var temp = new byte[stream.Length];
            stream.Read(temp, 0, temp.Length);
            _data = temp.ToReadOnlyCollection()!;
        }

        public WebhookImage(Stream stream)
        {
            Checks.CheckForNull(stream, nameof(stream));
            Checks.CheckForArgument(!stream.CanSeek || !stream.CanRead, nameof(stream));

            var temp = new byte[stream.Length - stream.Position];
            stream.Read(temp, 0, temp.Length);
            _data = temp.ToReadOnlyCollection()!;
        }

        public void Save(string path)
        {
            Checks.CheckForArgument(string.IsNullOrEmpty(path), nameof(path));
            File.WriteAllBytes(path, _data.ToArray());
        }

        public string ToUriScheme()
        {
            if (!string.IsNullOrEmpty(_uriCached))
                return _uriCached!;

            return _uriCached = $"data:image/png;base64,{Convert.ToBase64String(_data.ToArray())}";
        }
    }
}
