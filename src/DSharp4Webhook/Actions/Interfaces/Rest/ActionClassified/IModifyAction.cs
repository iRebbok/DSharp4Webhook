using DSharp4Webhook.Serialization;

namespace DSharp4Webhook.Actions.Rest
{
    /// <summary>
    ///     Webhook update action, name change, avatar change.
    /// </summary>
    public interface IModifyAction : IRestAction<IModifyResult>
    {
        /// <summary>
        ///     The serialized data contains name and avatar_url.
        /// </summary>
        public SerializeContext Context { get; }
    }
}
