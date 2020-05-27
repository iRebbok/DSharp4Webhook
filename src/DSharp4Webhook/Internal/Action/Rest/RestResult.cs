using DSharp4Webhook.Action;
using DSharp4Webhook.Rest;

namespace DSharp4Webhook.Internal
{
    internal class RestResult : IRestResult
    {
        public RestResponse? LastResponse { get => _responses != null && _responses.Length != 0 ? (RestResponse?)_responses[_responses.Length - 1] : null; }
        public RestResponse[] Responses { get => _responses; }

        private readonly RestResponse[] _responses;

        public RestResult(RestResponse[] responses)
        {
            _responses = responses;
        }
    }
}
