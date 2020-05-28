using DSharp4Webhook.Action;
using System;

namespace DSharp4Webhook.Internal
{
    /// <remarks>
    ///     Used for passing the callback to the action manager.
    /// </remarks>
    internal struct QueueActionContext
    {
        public IAction Action { get; }
        public Action<IResult> FirstCallback { get; }
        public Action<IResult, bool> SecondCallback { get; }
        public Action<bool> ThirdCallback { get; }
        public System.Action FourthCallback { get; }

        public QueueActionContext(IAction action)
        {
            Action = action;
            FirstCallback = null;
            SecondCallback = null;
            ThirdCallback = null;
            FourthCallback = null;
        }

        public QueueActionContext(IAction action, Action<IResult> callback) : this(action)
        {
            FirstCallback = callback;
        }

        public QueueActionContext(IAction action, Action<IResult, bool> callback) : this(action)
        {
            SecondCallback = callback;
        }

        public QueueActionContext(IAction action, Action<bool> callback) : this(action)
        {
            ThirdCallback = callback;
        }

        public QueueActionContext(IAction action, System.Action callback) : this(action)
        {
            FourthCallback = callback;
        }
    }
}
