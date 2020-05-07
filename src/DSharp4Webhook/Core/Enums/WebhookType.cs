namespace DSharp4Webhook.Core.Enums
{
    public enum WebhookType
    {
        /// <summary>
        ///     Unidentified type of webhook.
        /// </summary>
        NONE = 0,
        /// <summary>
        ///     Incoming Webhooks can post messages to channels with a generated token.
        /// </summary>
        INCOMING = 1,
        /// <summary>
        ///     Channel Follower Webhooks are internal webhooks used with Channel Following to post new messages into channels.
        /// </summary>
        /// <remarks>
        ///     It cannot be used as a normal webhook.
        /// </remarks>
        CHANNEL_FOLLOWER = 2,
    }
}
