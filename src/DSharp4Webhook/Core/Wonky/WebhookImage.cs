using DSharp4Webhook.Core;
using DSharp4Webhook.Util;
using DSharp4Webhook.Util.Extensions;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace DSharp4Webhook.Internal
{
    internal struct WebhookImage : IWebhookImage, IEquatable<IWebhookImage?>
    {
        /// <summary>
        ///     An empty image that will not modify the webhook.
        /// </summary>
        public static WebhookImage Empty { get; } = new WebhookImage();

        public ReadOnlyCollection<byte>? Data { get => _data; }

        private readonly ReadOnlyCollection<byte>? _data;
        private string? _uriCached;

        public WebhookImage(byte[] data)
        {
            Contract.AssertNotNull(data, nameof(data));
            _data = data.ToReadOnlyCollection()!;
            _uriCached = null;
        }

        public WebhookImage(FileInfo file)
        {
            Contract.AssertNotNull(file, nameof(file));
            Contract.AssertArgumentNotTrue(!file.Exists, nameof(file));
            using var stream = file.OpenRead();
            var temp = new byte[stream.Length];
            stream.Read(temp, 0, temp.Length);
            _data = temp.ToReadOnlyCollection()!;
            _uriCached = null;
        }

        public WebhookImage(Stream stream)
        {
            Contract.AssertNotNull(stream, nameof(stream));
            Contract.AssertArgumentNotTrue(!stream.CanSeek || !stream.CanRead, nameof(stream));

            var temp = new byte[stream.Length - stream.Position];
            stream.Read(temp, 0, temp.Length);
            _data = temp.ToReadOnlyCollection()!;
            _uriCached = null;
        }

        public void Save(string path)
        {
            Contract.AssertArgumentNotTrue(string.IsNullOrEmpty(path), nameof(path));
            File.WriteAllBytes(path, _data.ToArray());
        }

        public string ToUriScheme()
        {
            if (!string.IsNullOrEmpty(_uriCached))
                return _uriCached!;

            return _uriCached = $"data:image/png;base64,{Convert.ToBase64String(_data.ToArray())}";
        }

        public bool Equals(IWebhookImage? other)
        {
            return !(other is null) && Data == other.Data;
        }
    }
}
