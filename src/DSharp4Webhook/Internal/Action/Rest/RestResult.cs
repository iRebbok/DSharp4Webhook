using DSharp4Webhook.Action;
using DSharp4Webhook.Rest;
using DSharp4Webhook.Util.Extensions;
using System.Collections.ObjectModel;

namespace DSharp4Webhook.Internal
{
    internal class RestResult : IRestResult
    {
        public RestResponse? LastResponse { get => !(_responses is null) && _responses.Count != 0 ? (RestResponse?)_responses[_responses.Count - 1] : null; }
        public ReadOnlyCollection<RestResponse> Responses { get => _responses; }

        private readonly ReadOnlyCollection<RestResponse> _responses;

        public RestResult(RestResponse[] responses)
        {
            _responses = responses.ToReadOnlyCollection()!;
        }
    }
}
