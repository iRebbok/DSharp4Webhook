namespace DSharp4Webhook.Action
{
    /// <remarks>
    ///     Wrapper for actions that return nothing, like the status code 200.
    /// </remarks>
    public interface IRestAction : IRestAction<IRestResult> { }

    /// <summary>
    ///     Action related to rest.
    /// </summary>
    public interface IRestAction<TResult> : IAction<TResult> where TResult : IRestResult { }
}