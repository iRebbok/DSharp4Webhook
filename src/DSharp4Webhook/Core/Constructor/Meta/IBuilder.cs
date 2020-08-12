namespace DSharp4Webhook.Core.Constructor
{
    /// <summary>
    ///     Abstraction for constructors.
    /// </summary>
    public interface IBuilder<T>
    {
        /// <summary>
        ///     Resets the constructor to the default preset.
        /// </summary>
        public void Reset();

        public T Build();
    }
}
