using System;

namespace DSharp4Webhook.Core
{
    public interface IWebhookImage
    {
        /// <summary>
        ///     Image data.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        ///     Converts data to the uri format for transmitting images.
        /// </summary>
        public string ToUriScheme();

        /// <summary>
        ///     Saves an image.
        /// </summary>
        /// <param name="path">
        ///     The path to save the image to.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Path is null or empty.
        /// </exception>
        public void Save(string path);
    }
}
