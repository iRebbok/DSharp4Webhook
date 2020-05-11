using DSharp4Webhook.Action;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest;
using DSharp4Webhook.Util;
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

        public ActionManager(IWebhook webhook)
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
                    if (actionContext.Action?.IsExecuted ?? false)
                        continue;

                    //todo: logs
                    await FollowRateLimit(_limitInfo);
                    bool successfulness = false;

                    try { successfulness = await actionContext.Action?.ExecuteAsync(); }
                    catch { }

                    actionContext.FirstCallback?.Invoke(actionContext.Action?.GetResult(), successfulness);
                    actionContext.SecondCallback?.Invoke(successfulness);

                    OnActionExecuted?.Invoke(new ActionContext(actionContext.Action, successfulness));
                }
                else
                    await Task.Delay(150);
            }
        }

        public void Dispose()
        {
            _isntDisposed = false;

            // just take out the values until they run out
            while (_actions.TryDequeue(out _)) { }
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
                    await Task.Delay(mustWait);
                }
            }
        }

        public bool Start()
        {
            if (_isntDisposed && (_worker == null || _worker.IsCompleted || _worker.IsFaulted))
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

        public void Queue(IAction action, Action<IResult, bool> callback = null)
        {
            Checks.CheckForNull(action, nameof(action));
            _actions.Enqueue(new QueueActionContext(action, callback));
        }

        public void Queue(IAction action, Action<bool> callback = null)
        {
            Checks.CheckForNull(action, nameof(action));
            _actions.Enqueue(new QueueActionContext(action, callback));
        }
    }
}
