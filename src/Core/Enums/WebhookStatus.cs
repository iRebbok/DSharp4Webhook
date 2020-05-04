namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     Webhook statuses.
    ///     Used for internal processing of rest requests.
    /// </summary>
    public enum WebhookStatus
    {
        /// <summary>
        ///     Was not properly checked.
        ///     Has the potential to cause an error.
        /// </summary>
        NOT_CHECKED,
        /// <summary>
        ///     Has been verified to send requests safely.
        /// </summary>
        EXISTING,
        /// <summary>
        ///     Nonexistent, it is not safe to send requests.
        ///     These webhooks are immediately disposed of.
        /// </summary>
        NOT_EXISTING
    }
}
