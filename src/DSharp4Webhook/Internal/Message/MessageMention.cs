using DSharp4Webhook.Core;
using DSharp4Webhook.Core.Constructor;
using DSharp4Webhook.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSharp4Webhook.Internal
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class MessageMention : IMessageMention
    {
        private AllowedMention _allowedMention;
        private string[] _users;
        private string[] _roles;

        public AllowedMention AllowedMention { get => _allowedMention; }

        [JsonProperty(PropertyName = "parse")]
        public List<string> allowedMention
        {
            get
            {
                AllowedMention allowedResult = AllowedMention.NONE;

                // We use our own processing because the user can allow mutual exclusion
                // See https://discord.com/developers/docs/resources/channel#allowed-mentions-object-allowed-mentions-reference

                if (AllowedMention.HasFlag(AllowedMention.USERS) && _users.Length == 0)
                    allowedResult = AllowedMention.USERS;
                if (AllowedMention.HasFlag(AllowedMention.ROLES) && _roles.Length == 0)
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
        public string[] Users { get => _users; }

        [JsonProperty("roles")]
        public string[] Roles { get => _roles; }

        public MessageMention()
        {
            _allowedMention = AllowedMention.NONE;
            _users = Array.Empty<string>();
            _roles = Array.Empty<string>();
        }

        public MessageMention(AllowedMention mention) : this()
        {
            _allowedMention = mention;
        }

        public MessageMention(MessageMentionBuilder builder)
        {
            Checks.CheckForNull(builder, nameof(builder));

            _allowedMention = builder.AllowedMention;
            _users = builder.Users?.ToArray();
            _roles = builder.Roles?.ToArray();
        }
    }
}
