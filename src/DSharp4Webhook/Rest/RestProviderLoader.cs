using DSharp4Webhook.Core;
using DSharp4Webhook.Rest.Manipulation;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DSharp4Webhook.Rest
{
    /// <remarks>
    ///     We dynamically load the provider to ensure that dependencies are separated
    ///     and that you can create your own provider.
    /// </remarks>
    public static class RestProviderLoader
    {
        private static Type _provider;

        private static bool IsCorrectProvider(Type type)
        {
            Checks.CheckForNull(type, nameof(type));
            return type.IsSubclassOf(typeof(BaseRestProvider)) &&
                type.GetConstructor(new Type[] { typeof(IWebhook) }) != null;
        }

        /// <summary>
        ///     Gets the type of provider it that intends to use.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     If no suitable implementation is found.
        /// </exception>
        public static Type GetProviderType()
        {
            if (_provider == null)
                throw new InvalidOperationException("RestProvider does not have any implementation, make sure that you have included at least one of them in the build");

            return _provider;
        }

        /// <summary>
        ///     Sets the provider.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Type is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If the provider doesn't fit the rules.
        /// </exception>
        public static void SetProviderType(Type type)
        {
            Checks.CheckForNull(type, nameof(type));
            if (!IsCorrectProvider(type))
                throw new InvalidOperationException("This type of provider does not fit the rules");

            _provider = type;
        }

        /// <summary>
        ///     Creates a provider.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     If at least one of the parameters is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     If no suitable implementation is found.
        /// </exception>
        public static BaseRestProvider CreateProvider(IWebhook webhook)
        {
            Checks.CheckForNull(webhook, nameof(webhook));

            var providerType = GetProviderType();
            return Activator.CreateInstance(providerType, webhook) as BaseRestProvider;
        }
    }
}
