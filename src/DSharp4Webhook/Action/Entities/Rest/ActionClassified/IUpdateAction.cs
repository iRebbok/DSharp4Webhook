using DSharp4Webhook.Serialization;

namespace DSharp4Webhook.Action.Rest
{
    /// <summary>
    ///     Webhook update action, name change, avatar change.
    /// </summary>
    public interface IUpdateAction : IRestAction<IUpdateResult>
    {
        /// <summary>
        ///     The serialized data contains name and avatar_url.
        /// </summary>
        public SerializeContext Context { get; }
    }
}
