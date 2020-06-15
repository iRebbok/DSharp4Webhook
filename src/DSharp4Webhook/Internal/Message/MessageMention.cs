using DSharp4Webhook.Core;
using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Util;
using DSharp4Webhook.Util.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DSharp4Webhook.Internal
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class MessageMention : IMessageMention
    {
        private readonly AllowedMention _allowedMention;
        private readonly ReadOnlyCollection<string>? _users;
        private readonly ReadOnlyCollection<string>? _roles;

        public AllowedMention AllowedMention { get => _allowedMention; }

        [JsonProperty(PropertyName = "parse")]
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE1006 // Naming Styles
        private List<string> __allowedMention
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0051 // Remove unused private members
        {
            get
            {
                AllowedMention allowedResult = AllowedMention.NONE;

                // We use our own processing because the user can allow mutual exclusion
                // See https://discord.com/developers/docs/resources/channel#allowed-mentions-object-allowed-mentions-reference

                if (AllowedMention.HasFlag(AllowedMention.USERS) && _users?.Count == 0)
                    allowedResult = AllowedMention.USERS;
                if (AllowedMention.HasFlag(AllowedMention.ROLES) && _roles?.Count == 0)
                {
                    if (allowedResult != AllowedMention.NONE)
                        allowedResult |= AllowedMention.ROLES;
                    else
                        allowedResult = AllowedMention.ROLES;
                }
                if (AllowedMention.HasFlag(AllowedMention.EVERYONE))
                {
                    if (allowedResult != AllowedMention.NONE)
                        allowedResult |= AllowedMention.EVERYONE;
                    else
                        allowedResult = AllowedMention.EVERYONE;
                }

                List<string> allowedList = new List<string>();
                foreach (Enum flag in Enum.GetValues(typeof(AllowedMention)))
                    if (allowedResult.HasFlag(flag))
                        allowedList.Add(flag.ToString().ToLowerInvariant());

                return allowedList;
            }
        }

        [JsonProperty("users")]
        public ReadOnlyCollection<string>? Users { get => _users; }

        [JsonProperty("roles")]
        public ReadOnlyCollection<string>? Roles { get => _roles; }

        public MessageMention()
        {
            _allowedMention = AllowedMention.NONE;
        }

        public MessageMention(AllowedMention mention) : this()
        {
            _allowedMention = mention;
        }

        public MessageMention(MessageMentionBuilder builder)
        {
            Checks.CheckForNull(builder, nameof(builder));

            _allowedMention = builder.AllowedMention;
            _users = builder._users?.ToArray().ToReadOnlyCollection();
            _roles = builder._roles?.ToArray().ToReadOnlyCollection();
        }
    }
}
