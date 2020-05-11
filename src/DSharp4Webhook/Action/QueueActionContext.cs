using System;

namespace DSharp4Webhook.Action
{
    /// <remarks>
    ///     Used for passing the callback to the action manager.
    /// </remarks>
    public struct QueueActionContext
    {
        public IAction Action { get; }
        public Action<IResult, bool> FirstCallback { get; }
        public Action<bool> SecondCallback { get; }

        public QueueActionContext(IAction action)
        {
            Action = action;
            FirstCallback = null;
            SecondCallback = null;
        }

        public QueueActionContext(IAction action, Action<IResult, bool> callback) : this(action)
        {
            FirstCallback = callback;
        }

        public QueueActionContext(IAction action, Action<bool> callback) : this(action)
        {
            SecondCallback = callback;
        }
    }
}
