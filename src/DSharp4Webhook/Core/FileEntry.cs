using DSharp4Webhook.Buffers;
using DSharp4Webhook.Extensions;
using DSharp4Webhook.InternalExceptions;
using DSharp4Webhook.Util;
using System;
using System.IO;

namespace DSharp4Webhook.Serialization
{
    public struct FileEntry : IDisposable
    {
        private string _fileName;

        /// <summary>
        ///     Entry name.
        /// </summary>
        public string FileName
        {
            get => _fileName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("File name cannot be empty, whitespace or null", nameof(value));

                if (FileExtensions.NameIsInvalid(value))
                    throw new ArgumentException("File name is invalid", nameof(value));

                _fileName = value;
            }
        }

        /// <summary>
        ///     Entry content.
        /// </summary>
        public LongArraySegment<byte> Content { get; private set; }

        public FileEntry(FileInfo fileInfo)
        {
            Checks.CheckForNull(fileInfo, nameof(fileInfo));
            if (!fileInfo.Exists)
                throw new FileNotFoundException();
            else if (Checks.CheckForOversizeSafe(fileInfo.Length))
                throw new SizeOutOfRangeException();

            _fileName = fileInfo.Name;
            using var stream = fileInfo.OpenRead();
            Content = stream.ReadAsLongByteSegment();
        }

        public void SetContent(FileInfo fileInfo) => SetContent(fileInfo.FullName);

        public void SetContent(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            using var stream = File.OpenRead(path);
            SetContent(stream);
        }

        public void SetContent(Stream stream)
        {
            var newContent = stream.ReadAsLongByteSegment();
            if (Checks.CheckForOversizeSafe(newContent.Count))
            {
                LongArrayPool<byte>.Recycle(newContent.Array);
                throw new SizeOutOfRangeException();
            }

            Dispose();
            Content = newContent;
        }

        /// <summary>
        ///     Checks for file entry validatable.
        /// </summary>
        /// <returns>
        ///     true if the file entry is valid; otherwise, false.
        /// </returns>
        public bool IsValid() => !string.IsNullOrWhiteSpace(_fileName) && !(Content.Array is null) && !Checks.CheckForOversizeSafe(Content.Count);

        /// <summary>
        ///     Returns an array to its pool.
        /// </summary>
        public void Dispose()
        {
            LongArrayPool<byte>.Recycle(Content.Array);
            Content = default;
        }
    }
}
