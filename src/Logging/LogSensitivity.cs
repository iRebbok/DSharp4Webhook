namespace DSharp4Webhook.Logging
{
    /// <summary>
    ///     The sensitivity of the log.
    /// </summary>
    public enum LogSensitivity
    {
        /// <summary>
        ///     An error that stops sending a message or webhook itself from working.
        /// </summary>
        ERROR = 0,
        /// <summary>
        ///     A warning that may occur on one side of the interaction is essentially a processed error.
        /// </summary>
        WARN = 1,
        /// <summary>
        ///     Information that is used to track general flow.
        /// </summary>
        INFO = 2,
        /// <summary>
        ///     Very detailed logging information.
        /// </summary>
        DEBUG = 3,
        /// <summary>
        ///     Information about the library is most often needed for developers.
        /// </summary>
        VERBOSE = 4
    }
}
