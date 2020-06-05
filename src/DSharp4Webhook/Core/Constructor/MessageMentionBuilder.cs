using DSharp4Webhook.Internal;
using DSharp4Webhook.Util;
using System;
using System.Collections.Generic;

namespace DSharp4Webhook.Core.Constructor
{
    public sealed class MessageMentionBuilder : IBuilder
    {
        private AllowedMention _allowedMention;
        internal HashSet<string> _users;
        internal HashSet<string> _roles;

        #region Propeties

        /// <summary>
        ///     Allowed types of mentions.
        /// </summary>
        public AllowedMention AllowedMention
        {
            get => _allowedMention;
            set => _allowedMention = value;
        }

        /// <summary>
        ///     Users allowed to be mentioned.
        /// </summary>
        public HashSet<string> Users
        {
            get => _users ??= new HashSet<string>();
            set => _users = value;
        }

        /// <summary>
        ///     Roles allowed to be mentioned.
        /// </summary>
        public HashSet<string> Roles
        {
            get => _roles ??= new HashSet<string>();
            set => _roles = value;
        }

        #endregion

        private MessageMentionBuilder()
        {
            _allowedMention = AllowedMention.NONE;
        }

        private MessageMentionBuilder(AllowedMention mention)
        {
            _allowedMention = mention;
        }

        private MessageMentionBuilder(IWebhook webhook)
        {
            Checks.CheckForNull(webhook, nameof(webhook));
            _allowedMention = webhook.AllowedMention;
        }

        #region Static methods

        /// <summary>
        ///     Gets a new mention constructor.
        /// </summary>
        public static MessageMentionBuilder New() => new MessageMentionBuilder();

        /// <summary>
        ///     Gets a new mention constructor with a predefined allowed mention.
        /// </summary>
        public static MessageMentionBuilder New(AllowedMention mention) => new MessageMentionBuilder(mention);

        /// <summary>
        ///     Gets a new mention constructor with a preset of allowed mentions from the webhook.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="webhook"/> is null.
        /// </exception>
        public static MessageMentionBuilder New(IWebhook webhook) => new MessageMentionBuilder(webhook);

        #endregion

        #region Methods

        /// <summary>
        ///     Builds mentions.
        /// </summary>
        public IMessageMention Build()
        {
            return new MessageMention(this);
        }

        public void Reset()
        {
            _allowedMention = AllowedMention.NONE;
            _users = null;
            _roles = null;
        }

        #endregion
    }
}
