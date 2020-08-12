using DSharp4Webhook.Core;
using System;

namespace DSharp4Webhook.Serialization
{
    /// <summary>
    ///     Data serialization context that is used for the web.
    /// </summary>
    /// <remarks>
    ///     We support two types of content to send:
    ///         'application\json' - when we don't send files, just <see cref="Content"/> as json, then we can't send <see cref="Attachments"/>.
    ///         'multipart/form-data' - when we send files, then <see cref="Content"/> in 'payload_json', and it can be null,
    ///             <see cref="Content"/> it can be null if we only send the file.
    /// </remarks>
    public readonly struct SerializeContext : IValidable
    {
        /// <summary>
        ///     Type of serializing context.
        /// </summary>
        public SerializeType Type { get; }
        /// <summary>
        ///     Source data.
        /// </summary>
        /// <remarks>
        ///     Data to send, is the main one if used 'application/json'.
        /// </remarks>
        public byte[]? Content { get; }
        /// <summary>
        ///     Files: name-content
        /// </summary>
        public FileEntry[]? Attachments { get; }

        /// <summary>
        ///     Automatic format that is selected based on arguments.
        /// </summary>
        /// <param name="content">
        ///     Message content.
        /// </param>
        /// <param name="fileEntry">
        ///     One file entry.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     If the serialization type could not be determined/the data defining it is null.
        /// </exception>
        public SerializeContext(byte[]? content, FileEntry? fileEntry)
        {
            Type = SerializeType.APPLICATION_JSON;
            Content = content;
            Attachments = null;

            if (!(fileEntry is null))
            {
                Attachments = new FileEntry[1];
                Attachments[0] = fileEntry.Value;
                Type = SerializeType.MULTIPART_FORM_DATA;
            }
        }

        /// <summary>
        ///     Creates a type based on 'application/json'.
        /// </summary>
        /// <param name="content">
        ///     Content to send.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     The content is null.
        /// </exception>
        /// <remarks>
        ///     Content can't be null.
        /// </remarks>
        public SerializeContext(byte[] content)
        {
            Type = SerializeType.APPLICATION_JSON;
            Content = content;
            Attachments = null;
        }

        public bool IsValid() => Type != SerializeType.NULL && (!(Content is null) || !(Attachments is null));
    }
}
