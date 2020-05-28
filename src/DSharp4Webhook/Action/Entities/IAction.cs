using DSharp4Webhook.Core;
using DSharp4Webhook.Internal;
using System;
using System.Threading.Tasks;

namespace DSharp4Webhook.Action
{
    public interface IAction
    {
        /// <summary>
        ///     Source webhook where the action came from.
        /// </summary>
        public IWebhook Webhook { get; }

        /// <summary>
        ///     Has the action already been called.
        /// </summary>
        public bool IsExecuted { get; }

        /// <summary>
        ///     Inserts an action in the queue.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     The webhook is no longer able to process anything.
        /// </exception>
        public void Queue();

        /// <summary>
        ///     Inserts an action in the queue that calls a callback.
        /// </summary>
        /// <param name="callback">
        ///     Callback what is called after performing the action.
        /// </param>
        /// <remarks>
        ///     Closely related to <see cref="ExecuteAsync(Action{bool})"/>,
        ///     it passes control to <see cref="ActionManager"/>.
        /// </remarks>
        public void Queue(Action<bool> callback);

        /// <summary>
        ///     Everything is the same, just with result.
        /// </summary>
        public void Queue(Action<IResult, bool> callback);

        /// <summary>
        ///     Everything is the same, just without bool.
        /// </summary>
        public void Queue(Action<IResult> callback);

        /// <summary>
        ///     All the same, but without everything.
        /// </summary>
        public void Queue(System.Action callback);

        /// <summary>
        ///     Executes synchronously, blocking the current thread.
        /// </summary>
        /// <returns>
        ///     Result of the success of the action.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The webhook is no longer able to process anything.
        /// </exception>
        public bool Execute();

        /// <summary>
        ///     Asynchronous execution of an action.
        /// </summary>
        /// <returns>
        ///     Result of the success of the action.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     The webhook is no longer able to process anything.
        /// </exception>
        public Task<bool> ExecuteAsync();

        /// <summary>
        ///     Asynchronous execution of an action that calls a callback.
        /// </summary>
        /// <param name="callback">
        ///     Callback what is called after performing the action.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///     The webhook is no longer able to process anything.
        /// </exception>
        public Task ExecuteAsync(Action<bool> callback);

        /// <summary>
        ///     Everything is the same, only the result is added.
        /// </summary>
        public Task ExecuteAsync(Action<IResult, bool> callback);

        /// <summary>
        ///     Everything is the same, just without bool.
        /// </summary>
        public Task ExecuteAsync(Action<IResult> callback);

        /// <summary>
        ///     All the same, but without everything.
        /// </summary>
        public Task ExecuteAsync(System.Action callback);

        /// <summary>
        ///     Gets the result of the task or null.
        /// </summary>
        public IResult GetResult();
    }

    /// <summary>
    ///     Action that returns the result.
    /// </summary>
    public interface IAction<TResult> : IAction where TResult : IResult
    {
        /// <summary>
        ///     Result of the action.
        /// </summary>
        public TResult Result { get; }
    }
}
