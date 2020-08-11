using DSharp4Webhook.Util;
using System;

namespace DSharp4Webhook.Rest
{
    /// <summary>
    ///     Settings for rest request.
    /// </summary>
    /// <remarks>
    ///     This class shouldn't be sealed.
    /// </remarks>
    public class RestSettings
    {
        /// <summary>
        ///     The maximum number of attempts that can be used.
        ///     A zero value will be considered infinite.
        /// </summary>
        public uint MaxAttempts { get; set; }

        /// <summary>
        ///     Creates settings with the default value.
        /// </summary>
        public RestSettings()
        {
            MaxAttempts = 1;
        }

        /// <summary>
        ///     Creates settings and takes him as a parent.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     
        /// </exception>
        public RestSettings(RestSettings settings) : this()
        {
            Checks.CheckForNull(settings, nameof(settings));
            MaxAttempts = settings.MaxAttempts;
        }
    }
}
