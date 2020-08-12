using DSharp4Webhook.Internal;
using DSharp4Webhook.Util;
using System;

namespace DSharp4Webhook.Core.Constructor
{
    public sealed class ModifyContentBuilder : IBuilder
    {
        private string? _name;
        private IWebhookImage _image;

        #region Properties

        /// <summary>
        ///     Webhook name.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Attempt to assign a null value.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Exceeding the length limits.
        /// </exception>
        public string? Name
        {
            get => _name;
            set
            {
                if (value is null)
                    throw new ArgumentNullException("Value cannot be null", nameof(Name));

                value = value.Trim();
                Contract.CheckBounds(nameof(Name), $"Must be between {WebhookProvider.MIN_NICKNAME_LENGTH} and {WebhookProvider.MAX_NICKNAME_LENGTH} in length.", WebhookProvider.MAX_NICKNAME_LENGTH + 1, value.Length);
                Contract.CheckBoundsUnderside(nameof(Name), $"Must be between {WebhookProvider.MIN_NICKNAME_LENGTH} and {WebhookProvider.MAX_NICKNAME_LENGTH} in length.",
                    WebhookProvider.MIN_NICKNAME_LENGTH - 1, value.Length);
                _name = value;
            }
        }

        /// <summary>
        ///     Webhook image.
        /// </summary>
        /// <remarks>
        ///     Setting the value to null will break the image,
        ///     and it will return to the default value.
        /// </remarks>
        public IWebhookImage Image
        {
            get => _image;
            // Allowing null
            set => _image = value;
        }

        #endregion

        #region Static methods

        /// <summary>
        ///     Gets a new constructor.
        /// </summary>
        public static ModifyContentBuilder New() => new ModifyContentBuilder();

        #endregion

        private ModifyContentBuilder()
        {
            _image = WebhookImage.Empty;
        }

        /// <summary>
        ///     Building a modification content.
        /// </summary>
        public IModifyContent Build()
        {
            return new ModifyContent(this);
        }

        public void Reset()
        {
            _name = null;
            _image = WebhookImage.Empty;
        }
    }
}
