using DSharp4Webhook.Action;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest;
using System;
using System.Threading.Tasks;

namespace DSharp4Webhook.Internal
{
    public abstract class BaseRestAction<TResult> : IRestAction<TResult> where TResult : IRestResult
    {
        public TResult Result { get; protected set; }
        public IWebhook Webhook { get; }
        public bool IsExecuted { get; protected set; }
        public RestSettings RestSettings { get; }

        protected BaseRestAction(IWebhook webhook, RestSettings restSettings)
        {
            Webhook = webhook;
            RestSettings = restSettings;
        }

        public bool Execute()
        {
            return Task.Run(ExecuteAsync).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public IResult GetResult()
        {
            return Result;
        }

        public abstract Task<bool> ExecuteAsync();

        public async Task ExecuteAsync(Action<IResult> callback)
        {
            await ExecuteAsync();
            callback?.Invoke(Result);
        }

        public async Task ExecuteAsync(Action<IResult, bool> callback)
        {
            bool result = await ExecuteAsync();
            callback?.Invoke(Result, result);
        }

        public async Task ExecuteAsync(Action<bool> callback)
        {
            bool result = await ExecuteAsync();
            callback?.Invoke(result);
        }

        public async Task ExecuteAsync(System.Action callback)
        {
            await ExecuteAsync();
            callback?.Invoke();
        }

        public void Queue()
        {
            Webhook.ActionManager.Queue(this);
        }

        public void Queue(Action<bool> callback)
        {
            Webhook.ActionManager.Queue(this, callback);
        }

        public void Queue(Action<IResult, bool> callback)
        {
            Webhook.ActionManager.Queue(this, callback);
        }

        public void Queue(Action<IResult> callback)
        {
            Webhook.ActionManager.Queue(this, callback);
        }

        public void Queue(System.Action callback)
        {
            Webhook.ActionManager.Queue(this, callback);
        }

        public void SettingRateLimit()
        {
            if (!(Result is null) && Result.LastResponse.HasValue)
                Webhook.ActionManager.SetRateLimit(Result.LastResponse.Value.RateLimit);
        }

        protected void CheckExecution()
        {
            if (IsExecuted)
                throw new InvalidOperationException("The action has already been performed");
            IsExecuted = true;
        }
    }
}
