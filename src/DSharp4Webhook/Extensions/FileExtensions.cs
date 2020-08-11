using System.IO;
using System.Text.RegularExpressions;

namespace DSharp4Webhook.Extensions
{
    public static class FileExtensions
    {
        private static readonly Regex _invalidNameRegex = new Regex($"[{Regex.Escape(new string(Path.GetInvalidPathChars()))}]",
            RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        ///     Check the file name.
        /// </summary>
        /// <returns>
        ///     true if is invalid; otherwise, false.
        /// </returns>
        public static bool NameIsInvalid(string filename) => _invalidNameRegex.IsMatch(filename);
    }
}
