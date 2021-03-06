namespace DSharp4Webhook.Actions
{
    public readonly struct ActionContext
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
