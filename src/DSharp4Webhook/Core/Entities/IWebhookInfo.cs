using DSharp4Webhook.Core.Enums;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     Received information about webhook in a direct GET request.
    /// </summary>
    /// <remarks>
    ///     The implementation is located in <see cref="WebhookInfo"/>.
    /// </remarks>
    public interface IWebhookInfo
    {
        /// <summary>
        ///     The type of webhook we are dealing with.
        /// </summary>
        WebhookType Type { get; }

        /// <summary>
        ///     Snowflake webhook id.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Snowflake converted to <see cref="ulong"/>.
        /// </summary>
        ulong IdULong { get; }

        /// <summary>
        ///     Webhook name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Avatar id for webhook.
        /// </summary>
#nullable enable
        string AvatarId { get; }

        /// <summary>
        ///     Gets the avatar url for webhook.
        /// </summary>
        string AvatarUrl { get; }

        /// <summary>
        ///     Snowflake webhook id of the channel it interacts with.
        /// </summary>
        string ChannelId { get; }

        /// <summary>
        ///     Snowflake converted to <see cref="ulong"/>.
        /// </summary>
        ulong ChannelIdUlong { get; }

        /// <summary>
        ///     Snowflake webhook guild id where it is hosted.
        /// </summary>
        string GuildId { get; }

        /// <summary>
        ///     Snowflake converted to <see cref="ulong"/>.
        /// </summary>
        ulong GuildIdULong { get; }

        /// <summary>
        ///     Webhook token.
        /// </summary>
        string Token { get; }
    }
}
