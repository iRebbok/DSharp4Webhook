using DSharp4Webhook.Actions;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest;
using DSharp4Webhook.Util;
using DSharp4Webhook.Util.Extensions;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DSharp4Webhook.Internal
{
    internal sealed class ActionManager : IActionManager
    {
        public event Action<ActionContext> OnActionExecuted;

        public IWebhook Webhook { get => _webhook; }
        public RateLimitInfo? LatestLimitInfo { get => _limitInfo; }
        public Task Worker { get => _worker; }

        // Tracks dispose.
        private bool _isntDisposed;
        private RateLimitInfo? _limitInfo;
        private Task _worker;

        private readonly IWebhook _webhook;
        private readonly ConcurrentQueue<QueueActionContext> _actions;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public ActionManager(IWebhook webhook)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            _webhook = webhook;

            _isntDisposed = true;
            _limitInfo = null;
            _actions = new ConcurrentQueue<QueueActionContext>();

            // Immediately launch the worker
            Start();
        }

        private async Task Do()
        {
            while (_isntDisposed && _webhook.Status != WebhookStatus.NOT_EXISTING)
            {
                if (_actions.TryDequeue(out var actionContext))
                {
                    // Skips completed actions
                    if (actionContext.Action?.IsExecuted ?? true)
                        continue;

                    //todo: logs
                    await FollowRateLimit(_limitInfo).ConfigureAwait(false);
                    bool successfulness = false;

                    try { successfulness = await actionContext.Action?.ExecuteAsync()!; }
                    catch { }

                    if (!(actionContext.FirstCallback is null))
                        EventUtil.HandleSafely(false, actionContext.FirstCallback.Method, actionContext.FirstCallback.Target, actionContext.Action?.GetResult()!);
                    else if (!(actionContext.SecondCallback is null))
                        EventUtil.HandleSafely(false, actionContext.SecondCallback.Method, actionContext.SecondCallback.Target, actionContext.Action?.GetResult()!, successfulness);
                    else if (!(actionContext.ThirdCallback is null))
                        EventUtil.HandleSafely(false, actionContext.ThirdCallback.Method, actionContext.ThirdCallback.Target, successfulness);
                    else if (!(actionContext.FourthCallback is null))
                        EventUtil.HandleSafely(false, actionContext.FourthCallback.Method, actionContext.FourthCallback.Target);
                    else
                        OnActionExecuted.InvokeSafely(false, new ActionContext(actionContext.Action, successfulness));
                }
                else
                    await Task.Delay(150).ConfigureAwait(false);
            }
        }

        public void Dispose()
        {
            _isntDisposed = false;

            // just take out the values until they run out
            while (_actions.TryDequeue(out _)) { }

            GC.SuppressFinalize(this);
        }

        public void SetRateLimit(RateLimitInfo rateLimit)
        {
            lock (this)
                _limitInfo = rateLimit;
        }

        public async Task FollowRateLimit(RateLimitInfo? rateLimit)
        {
            if (rateLimit.HasValue)
            {
                TimeSpan mustWait = rateLimit.Value.MustWait;
                if (mustWait != TimeSpan.Zero)
                {
                    //todo: logs
                    await Task.Delay(mustWait).ConfigureAwait(false);
                }
            }
        }

        public bool Start()
        {
            if (_isntDisposed && (_worker is null || _worker.IsCompleted || _worker.IsFaulted))
            {
                //todo: logs
                _worker = Task.Run(Do);
                return true;
            }
            return false;
        }

        public void Queue(IAction action)
        {
            Checks.CheckForNull(action, nameof(action));
            _actions.Enqueue(new QueueActionContext(action));
        }

        public void Queue(IAction action, Action<IResult> callback)
        {
            Checks.CheckForNull(action, nameof(action));
            _actions.Enqueue(new QueueActionContext(action, callback));
        }

        public void Queue(IAction action, Action<IResult, bool> callback)
        {
            Checks.CheckForNull(action, nameof(action));
            _actions.Enqueue(new QueueActionContext(action, callback));
        }

        public void Queue(IAction action, Action<bool> callback)
        {
            Checks.CheckForNull(action, nameof(action));
            _actions.Enqueue(new QueueActionContext(action, callback));
        }

        public void Queue(IAction action, System.Action callback)
        {
            Checks.CheckForNull(action, nameof(action));
            _actions.Enqueue(new QueueActionContext(action, callback));
        }
    }
}
