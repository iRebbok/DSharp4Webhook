using DSharp4Webhook.Rest.Mono.Util;
using DSharp4Webhook.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace DSharp4Webhook.Rest.Mono
{
    /// <summary>
    ///     Auxiliary class for working with the 'multipart/form-data' data stream.
    /// </summary>
    /// <remarks>
    ///     Was stolen from a solution here, but much refactoring has been done ¯\_(ツ)_/¯
    ///     https://www.codeproject.com/questions/896600/how-can-upload-files-using-http-web-request-in-win
    /// </remarks>
    public static class MultipartHelper
    {
        // DO NOT USE Encoding.UTF8
        // This took a little over an hour to debug.
        // If you use Encoding.UTF8 with StreamWriter,
        // it adds the preamble (UTF8-BOM) which adds a '?' at the beginning of the data.
        private static readonly Encoding encoding = new UTF8Encoding();

        /// <summary>
        ///     Shortcut for a carriage return, line feed.
        /// </summary>
        private const string crlf = "\r\n";

        /// <summary>
        ///     Supertemplate (template of a template) of a file header.
        ///     <para>
        ///         Supertemplate -> template:
        ///         0: boundary
        ///     </para>
        ///     <para>
        ///         Template -> header:
        ///         0: index
        ///         1: file name
        ///     </para>
        /// </summary>
        private const string fileHeaderSupertemplate = "--{0}" + crlf +
                                                    "Content-Disposition: form-data; name=\"file{{0}}\"; filename=\"{{1}}\"" + crlf +
                                                    "Content-Type: application/octet-stream" + crlf +
                                                    crlf;

        /// <summary>
        ///     Template of a content header.
        ///     <para>
        ///         Template -> header:
        ///         0: boundary
        ///         1: content name
        ///         2: content type (MIME)
        ///     </para>
        /// </summary>
        private const string contentHeaderTemplate = "--{0}" + crlf +
                                                "Content-Disposition: form-data; name=\"{1}\"" + crlf +
                                                "Content-Type: {2}" + crlf +
                                                crlf;

        /// <summary>
        ///     Template of the footer.
        ///     <para>
        ///         Template -> header:
        ///         0: boundary
        ///     </para>
        /// </summary>
        private const string footerTemplate = "--{0}--" + crlf;

        public static void PrepareMultipartFormDataRequest(HttpWebRequest request, Stream requestStream, SerializeContext context)
        {
            string boundary = Guid.NewGuid().ToString();
            request.ContentType = "multipart/form-data; boundary=" + boundary;

            var content = GetMultipartFormData(context.Files!, context.Content.ToArray(), boundary);
            requestStream.Write(content, 0, content.Length);
        }

        private static void WriteParameter(Stream data, TextWriter text, string header, byte[] content)
        {
            // Begin with header.
            text.Write(header);

            // Insert content.
            data.Write(content, 0, content.Length);

            // Finish with 2 newlines.
            // Delimits this parameter from the next parameter OR the content OR the footer.
            text.Write(crlf + crlf);
        }

        private static void WriteFiles(Stream data, TextWriter text, string boundary, IDictionary<string, ReadOnlyCollection<byte>> files)
        {
            // Bake boundary into header template.
            string headerTemplate = string.Format(fileHeaderSupertemplate, boundary);
            int index = 0;

            foreach ((string fileName, var fileContent) in files)
            {
                string header = string.Format(headerTemplate, index, fileName);
                WriteParameter(data, text, header, fileContent.ToArray());

                index++;
            }
        }

        // HACK: this assumes the content is JSON. Currently, SerializeContext does not have the content type (aside from the useless SerializeType). Until the content type is
        // provided, it is assumed to be JSON.
        private static void WriteContent(Stream data, TextWriter text, string boundary, byte[] content)
        {
            string header = string.Format(contentHeaderTemplate, boundary, "payload_json", "application/json");

            WriteParameter(data, text, header, content);
        }

        private static void WriteFooter(TextWriter text, string boundary)
        {
            // Add the end of the request.
            // Last parameter already CRLFs the start of this.
            text.Write(footerTemplate, boundary);
        }

        private static byte[] GetMultipartFormData(IDictionary<string, ReadOnlyCollection<byte>> files, byte[] content, string boundary)
        {
            using var data = new MemoryStream();
            using var text = new StreamWriter(data, encoding)
            {
                AutoFlush = true,
                NewLine = crlf
            };

            if (files.Count > 0)
            {
                WriteFiles(data, text, boundary, files);
            }

            if (!(content is null))
            {
                WriteContent(data, text, boundary, content);
            }

            WriteFooter(text, boundary);

            return data.ToArray();
        }
    }
}
