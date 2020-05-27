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

        public async Task ExecuteAsync(Action<TResult, bool> callback)
        {
            bool result = await ExecuteAsync();
            callback?.Invoke(Result, result);
        }

        public async Task ExecuteAsync(Action<bool> callback)
        {
            bool result = await ExecuteAsync();
            callback?.Invoke(result);
        }

        public void Queue(Action<TResult, bool> callback)
        {
            Webhook.ActionManager.Queue(this, callback as Action<IResult, bool>);
        }

        public void Queue()
        {
            Webhook.ActionManager.Queue(this);
        }

        public void Queue(Action<bool> callback)
        {
            Webhook.ActionManager.Queue(this, callback);
        }

        public void SettingRateLimit()
        {
            if (Result != null && Result.LastResponse.HasValue)
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
