using System;

namespace DSharp4Webhook.Core
{
    /// <summary>
    ///     Permissions for mentions to be used.
    ///     <para>
    ///         See for more information:
    ///         <c>https://discord.com/developers/docs/resources/channel#allowed-mentions-object-allowed-mention-types</c>
    ///     </para>
    /// </summary>
    [Flags]
    public enum AllowedMention
    {
        /// <summary>
        ///     Prohibits any mentions.
        /// </summary>
        NONE = -1,
        /// <summary>
        ///     Allows roles to be mentioned.
        /// </summary>
        ROLES = 1 << 0,
        /// <summary>
        ///     Allows users to be mentioned.
        /// </summary>
        USERS = 1 << 1,
        /// <summary>
        ///     Allows @everyone and @here to be mentioned.
        /// </summary>
        EVERYONE = 1 << 2
    }
}
