using System;

namespace DSharp4Webhook.Rest.Manipulation
{
    /// <summary>
    ///     Indicates the priority of the provider,
    ///     we select the provider with a higher priority if there are several of them.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ProviderPriorityAttribute : Attribute
    {
        public byte Priority { get; }

        public ProviderPriorityAttribute(byte priority)
        {
            Priority = priority;
        }
    }
}
