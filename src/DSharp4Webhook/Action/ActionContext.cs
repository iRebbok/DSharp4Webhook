namespace DSharp4Webhook.Action
{
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public readonly struct ActionContext
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        /// <summary>
        ///     Executed action.
        /// </summary>
        public IAction Action { get; }

        /// <summary>
        ///     Success of the action.
        /// </summary>
        public bool IsSuccessfully { get; }

        public ActionContext(IAction action, bool isSecessfully)
        {
            Action = action;
            IsSuccessfully = isSecessfully;
        }
    }
}
