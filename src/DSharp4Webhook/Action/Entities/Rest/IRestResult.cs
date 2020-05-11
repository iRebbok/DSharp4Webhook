using DSharp4Webhook.Rest;

namespace DSharp4Webhook.Action
{
    /// <summary>
    ///     Result of the rest action.
    /// </summary>
    public interface IRestResult : IResult
    {
        /// <summary>
        ///     Returns the last rest response.
        /// </summary>
        public RestResponse? LastResponse { get; }

        /// <summary>
        ///     All responses to rest queries.
        /// </summary>
        public RestResponse[] Responses { get; }
    }
}
