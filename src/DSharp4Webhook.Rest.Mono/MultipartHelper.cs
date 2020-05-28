using DSharp4Webhook.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DSharp4Webhook.Rest.Mono
{
    /// <summary>
    ///     Auxiliary class for working with the 'multipart/form-data' data stream.
    /// </summary>
    /// <remarks>
    ///     Was stolen from a solution here ¯\_(ツ)_/¯
    ///     https://www.codeproject.com/questions/896600/how-can-upload-files-using-http-web-request-in-win
    /// </remarks>
    public static class MultipartHelper
    {
        private static readonly Encoding Encoding = Encoding.UTF8;
        private const string formDataFileTemplate = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n";
        private const string formDataTemplate = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";

        public static void PrepareMultipartFormDataRequest(HttpWebRequest request, Stream requestStream, SerializeContext context)
        {
            string formDataBoundary = string.Format("----------{0:N}", Guid.NewGuid());
            request.ContentType = "multipart/form-data; boundary=" + formDataBoundary;
            var content = GetMultipartFormData(context.Files, context.Content, formDataBoundary);
            requestStream.Write(content, 0, content.Length);
        }

        private static byte[] GetMultipartFormData(Dictionary<string, byte[]> files, byte[] content, string boundary)
        {
            using var formDataStream = new MemoryStream();
            bool needsCLRF = false;

            int index = -1;
            foreach (var pair in files)
            {
                index++;

                // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    formDataStream.Write(Encoding.GetBytes("\r\n"), 0, Encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                // Add just the first part of this param, since we will write the file data directly to the Stream
                string header = string.Format(formDataFileTemplate,
                    boundary, $"file{index}", pair.Key, "application/octet-stream");
                formDataStream.Write(Encoding.GetBytes(header), 0, Encoding.GetByteCount(header));

                // Write the file data directly to the Stream, rather than serializing it to a string.
                formDataStream.Write(pair.Value, 0, pair.Value.Length);
            }

            if (content != null)
            {
                // Putting a header between the last file
                formDataStream.Write(Encoding.GetBytes("\r\n"), 0, Encoding.GetByteCount("\r\n"));

                // Add just the first part of this param, since we will write the file data directly to the Stream
                string header = string.Format(formDataTemplate,
                    boundary, $"payload_json", "application/json");
                formDataStream.Write(Encoding.GetBytes(header), 0, Encoding.GetByteCount(header));

                // Writing content to the stream
                formDataStream.Write(content, 0, content.Length);
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(Encoding.GetBytes(footer), 0, Encoding.GetByteCount(footer));

            return formDataStream.ToArray();
        }
    }
}
