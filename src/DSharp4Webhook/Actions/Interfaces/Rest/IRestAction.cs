using DSharp4Webhook.Rest;

namespace DSharp4Webhook.Actions
{
    /// <remarks>
    ///     Wrapper for actions that return nothing, like the status code 200.
    /// </remarks>
    public interface IRestAction : IRestAction<IRestResult> { }

    /// <summary>
    ///     Action related to rest.
    /// </summary>
    public interface IRestAction<TResult> : IAction<TResult> where TResult : IRestResult
    {
        /// <summary>
        ///     Rest settings that the current action will be performed with.
        /// </summary>
        public RestSettings RestSettings { get; }
    }
}
