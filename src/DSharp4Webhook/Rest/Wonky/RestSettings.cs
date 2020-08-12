namespace DSharp4Webhook.Rest
{
    /// <summary>
    ///     Settings for rest request.
    /// </summary>
    /// <remarks>
    ///     This class shouldn't be sealed.
    /// </remarks>
    public struct RestSettings
    {
        /// <summary>
        ///     Attempts at a zero-count value after the first attempt.
        /// </summary>
        public byte Attempts { get; set; }

        /// <summary>
        ///     Duplicates settings.
        /// </summary>
        public RestSettings(RestSettings settings)
        {
            Attempts = settings.Attempts;
        }
    }
}
