namespace DSharp4Webhook.Core
{
    public interface IValidable
    {
        /// <summary>
        ///     Checks validity.
        /// </summary>
        /// <returns>
        ///     true if is valid; otherwise, false.
        /// </returns>
        public bool IsValid();
    }
}
