using DSharp4Webhook.Core;
using DSharp4Webhook.Rest;
using System;
using System.Threading.Tasks;

namespace DSharp4Webhook.Action
{
    /// <summary>
    ///     Action manager for webhook.
    /// </summary>
    public interface IActionManager : IDisposable
    {
        /// <summary>
        ///     Source webhook.
        /// </summary>
        public IWebhook Webhook { get; }

        /// <summary>
        ///     The latest <see cref="RateLimitInfo"/> from rest action.
        /// </summary>
        public RateLimitInfo? LatestLimitInfo { get; }

        /// <summary>
        ///     Worker's task.
        /// </summary>
        public Task Worker { get; }

        /// <summary>
        ///     Starts worker.
        /// </summary>
        /// <returns>
        ///     The success of the startup,
        ///     true if worker was started,
        ///     otherwise false.
        /// </returns>
        public bool Start();

        /// <summary>
        ///     Puts an action in the queue for execution.
        /// </summary>
        /// <param name="action">
        ///     Action.
        /// </param>
        public void Queue(IAction action);

        /// <summary>
        ///     Puts the action in the queue for execution
        ///     and calls the callback for the function execution.
        /// </summary>
        public void Queue(IAction action, Action<bool> callback = null);

        /// <summary>
        ///     Puts the action in the queue for execution
        ///     and calls the callback for the function execution.
        /// </summary>
        public void Queue(IAction action, Action<IResult, bool> callback = null);

        /// <summary>
        ///     Sets <see cref="RateLimitInfo"/> as the latest.
        /// </summary>
        public void SetRateLimit(RateLimitInfo rateLimit);

        /// <summary>
        ///     Follows ratelimit if necessary.
        /// </summary>
        public Task FollowRateLimit(RateLimitInfo? rateLimit);

        /// <summary>
        ///     Event when the action is performed by IActionManager.
        /// </summary>
        public event Action<ActionContext> OnActionExecuted;
    }
}
