namespace DSharp4Webhook.Core.Serialization
{
    /// <summary>
    ///     Indicates whether webhook data can be serialized.
    /// </summary>
    public interface IWSerializable
    {
        /// <summary>
        ///     Serializes data to a type format.
        /// </summary>
        SerializeContext Serialize();
    }
}
