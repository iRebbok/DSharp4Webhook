namespace DSharp4Webhook.Core.Embed.Subtypes
{
    public interface IIconable
    {
        /// <summary>
        ///     Url of icon.
        /// </summary>
#nullable enable
        public string? IconUrl { get; }
#nullable restore

        /// <summary>
        ///     A proxied url of icon.
        /// </summary>
#nullable enable
        public string? ProxyIconUrl { get; }
#nullable restore
    }
}
