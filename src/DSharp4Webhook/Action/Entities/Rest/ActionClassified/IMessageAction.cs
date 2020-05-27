using DSharp4Webhook.Core;

namespace DSharp4Webhook.Action.Rest
{
    /// <summary>
    ///     Sending a message action.
    /// </summary>
    public interface IMessageAction : IRestAction
    {
        public IMessage Message { get; }
    }
}
