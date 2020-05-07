using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

using DSharp4Webhook.Rest.Manipulation;
using DSharp4Webhook.Util;

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
                type.GetConstructor(new Type[] { typeof(RestClient), typeof(SemaphoreSlim) }) != null;
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
        public static BaseRestProvider CreateProvider(RestClient restClient, SemaphoreSlim locker)
        {
            Checks.CheckForNull(restClient, nameof(restClient));
            Checks.CheckForNull(locker, nameof(locker));

            var providerType = GetProviderType();
            return Activator.CreateInstance(providerType, restClient, locker) as BaseRestProvider;
        }

        private class RestProviderComparer : IComparer<Type>
        {
            public int Compare(Type x, Type y)
            {
                byte primaryPriority = x.GetCustomAttribute<ProviderPriorityAttribute>()?.Priority ?? 0;
                byte secondaryPriority = y.GetCustomAttribute<ProviderPriorityAttribute>()?.Priority ?? 0;

                return secondaryPriority.CompareTo(primaryPriority);
            }
        }
    }
}
