using DSharp4Webhook.Rest;
using System.Collections.ObjectModel;

namespace DSharp4Webhook.Actions
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
        public ReadOnlyCollection<RestResponse>? Responses { get; }
    }
}
