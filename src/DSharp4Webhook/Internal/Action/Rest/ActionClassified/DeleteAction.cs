using DSharp4Webhook.Action;
using DSharp4Webhook.Action.Rest;
using DSharp4Webhook.Core;
using DSharp4Webhook.Rest.Manipulation;
using System.Linq;
using System.Threading.Tasks;

namespace DSharp4Webhook.Internal
{
    internal sealed class DeleteAction : BaseRestAction<IRestResult>, IDeleteAction
    {
        public DeleteAction(IWebhook webhook) : base(webhook) { }

        public override async Task<bool> ExecuteAsync()
        {
            CheckExecution();
            Result = new RestResult(await Webhook.RestProvider.DELETE(Webhook.GetWebhookUrl(), 1));
            Webhook.Dispose();
            return Result.LastResponse.HasValue && BaseRestProvider.DELETE_ALLOWED_STATUSES.Contains(Result.LastResponse.Value.StatusCode);
        }
    }
}
