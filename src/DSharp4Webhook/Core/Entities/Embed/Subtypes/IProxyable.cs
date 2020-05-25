namespace DSharp4Webhook.Core.Embed.Subtypes
{
    public interface IProxyable
    {
        /// <summary>
        ///     A proxied url.
        /// </summary>
#nullable enable
        public string? ProxyUrl { get; }
#nullable restore
    }
}
