using DSharp4Webhook.Util;
using DSharp4Webhook.Util.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DSharp4Webhook.Serialization
{
    /// <summary>
    ///     Data serialization context that is used for the web.
    /// </summary>
    /// <remarks>
    ///     We support two types of content to send:
    ///         'application\json' - when we don't send files, just <see cref="Content"/> as json, then we can't send <see cref="Files"/>.
    ///         'multipart/form-data' - when we send files, then <see cref="Content"/> in 'payload_json', and it can be null,
    ///             <see cref="Content"/> it can be null if we only send the file.
    /// </remarks>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct SerializeContext
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        /// <summary>
        ///     Type of serializing context.
        /// </summary>
        public SerializeType Type { get; private set; }
        /// <summary>
        ///     Source data.
        /// </summary>
        /// <remarks>
        ///     Data to send, is the main one if used 'application/json'.
        /// </remarks>
        public ReadOnlyCollection<byte>? Content { get; }
        /// <summary>
        ///     Files: name-content
        /// </summary>
        public Dictionary<string, ReadOnlyCollection<byte>>? Files { get; private set; }
        /// <summary>
        ///     Automatic format that is selected based on arguments.
        /// </summary>
        /// <param name="content">
        ///     Message content.
        /// </param>
        /// <param name="data">
        ///     File contents.
        /// </param>
        /// <param name="fileName">
        ///     Name of the file to send, if null then new guid.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     If the serialization type could not be determined/the data defining it is null.
        /// </exception>
        public SerializeContext(byte[]? content, byte[]? data, string? fileName = null)
        {
            fileName = string.IsNullOrWhiteSpace(fileName) ? Guid.NewGuid().ToString() : fileName;

            // Then reassign if necessary
            Files = null;

            if (!(data is null))
            {
                Type = SerializeType.MULTIPART_FORM_DATA;
#pragma warning disable CS8604 // Possible null reference argument.
                Files = new Dictionary<string, ReadOnlyCollection<byte>> { [fileName] = new ReadOnlyCollection<byte>(data) };
#pragma warning restore CS8604 // Possible null reference argument.
            }
            else if (!(content is null))
                Type = SerializeType.APPLICATION_JSON;
            else
                throw new InvalidOperationException("Data is not defined rightly");

            Content = content.ToReadOnlyCollection();
        }

        /// <summary>
        ///     Creates an object with already serialized data.
        /// </summary>
        public SerializeContext(byte[]? content, IDictionary<string, ReadOnlyCollection<byte>>? files = null)
        {
            Type = files is null || (files?.Count ?? 0) < 1 ? SerializeType.APPLICATION_JSON : SerializeType.MULTIPART_FORM_DATA;
            Content = content.ToReadOnlyCollection();
            Files = files.ToDictionary(k => k.Key, v => v.Value);
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
            Checks.CheckForNull(content, nameof(content));

            Type = SerializeType.APPLICATION_JSON;
            Content = content.ToReadOnlyCollection();

            Files = null;
        }

        /// <summary>
        ///     Creates a type based on 'multipart/form-data'.
        /// </summary>
        /// <param name="data">
        ///     File data to send.
        /// </param>
        /// <param name="fileName">
        ///     Name of the file to send, if null then new guid.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     The file content is null.
        /// </exception>
        /// <remarks>
        ///     The file content cannot be null.
        /// </remarks>
        public SerializeContext(byte[] data, string? fileName = null)
        {
            Checks.CheckForNull(data, nameof(data));
            fileName = string.IsNullOrWhiteSpace(fileName) ? Guid.NewGuid().ToString() : fileName;

            Type = SerializeType.MULTIPART_FORM_DATA;
#pragma warning disable CS8604 // Possible null reference argument.
            Files = new Dictionary<string, ReadOnlyCollection<byte>> { [fileName] = new ReadOnlyCollection<byte>(data) };
#pragma warning restore CS8604 // Possible null reference argument.

            Content = null;
        }

        /// <summary>
        ///     Adds a file to the serialized query.
        /// </summary>
        /// <param name="data">
        ///     File content.
        /// </param>
        /// <param name="fileName">
        ///     File name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     File content is null.
        /// </exception>
        public void AddFile(byte[] data, string? fileName = null)
        {
            Checks.CheckForNull(data, nameof(data));

            fileName = string.IsNullOrWhiteSpace(fileName) ? Guid.NewGuid().ToString() : fileName;

            if (Files is null)
            {
                Files = new Dictionary<string, ReadOnlyCollection<byte>>();
                // The format changes when a new file is added
                Type = SerializeType.MULTIPART_FORM_DATA;
            }
            // Setting the value forcibly even if it exists
#pragma warning disable CS8604 // Possible null reference argument.
            Files[fileName] = new ReadOnlyCollection<byte>(data);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        /// <summary>
        ///     Deletes a file from the list.
        /// </summary>
        /// <param name="fileName">
        ///     File name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     File name is null.
        /// </exception>
        /// <returns>
        ///     true if the file was deleted from the collection;
        ///     otherwise false.
        /// </returns>
        public bool RemoveFile(string fileName)
        {
            Checks.CheckForNull(fileName, nameof(fileName));

            return Files?.Remove(fileName) ?? false;
        }

        public void RemoveFiles()
        {
            Files?.Clear();
        }
    }
}
