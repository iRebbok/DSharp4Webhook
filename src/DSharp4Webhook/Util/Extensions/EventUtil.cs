using System;
using System.Reflection;

namespace DSharp4Webhook.Util.Extensions
{
    /// <summary>
    ///     Auxiliary tools for events.
    /// </summary>
    internal static class EventUtil
    {
        /// <summary>
        ///     Executes all delegate listeners safely.
        /// </summary>
        internal static void InvokeSafely(this MulticastDelegate action, bool fromLog = false, params object[] args)
        {
            // Just return, this will allow you not to use null-conditional operator at each event
            if (action is null)
                return;

            foreach (var handler in action.GetInvocationList())
            {
                HandleSafely(fromLog, handler.Method, handler.Target, args);
            }
        }

        /// <summary>
        ///     Executes <see cref="MethodInfo"/> safely.
        /// </summary>
        /// <param name="instance">
        ///     Instance of the delegate method object,
        ///     null is used in the case of a static method.
        /// </param>
        internal static void HandleSafely(bool fromLog, MethodInfo action, object instance, params object[] args)
        {
            try
            {
                action.Invoke(instance, args);
            }
            // todo: webhook logs
            catch (System.Exception) { }
        }
    }
}
