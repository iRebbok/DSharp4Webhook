namespace DSharp4Webhook.Core.Embed.Subtypes
{
    /// <remarks>
    ///     Most embed objects include the ability to change the size of data,
    ///     this is a simple abstraction for them.
    /// </remarks>
    public interface IResizable
    {
        public uint? Height { get; }

        public uint? Width { get; }
    }
}
