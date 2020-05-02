using DSharp4Webhook.Entities;

namespace DSharp4Webhook.Util
{
    public static class Merger
    {
        /// <summary>
        ///     Combines data about their absence.
        /// </summary>
        public static IBaseWebhookData Merge(IBaseWebhookData from, IBaseWebhookData to)
        {
            if (string.IsNullOrEmpty(to.Username) && !string.IsNullOrEmpty(from.Username))
                to.Username = from.Username;
            if (string.IsNullOrEmpty(to.AvatarUrl) && !string.IsNullOrEmpty(from.AvatarUrl))
                to.AvatarUrl = from.AvatarUrl;
            return to;
        }
    }
}
