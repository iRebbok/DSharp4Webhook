using DSharp4Webhook.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DSharp4Webhook.Internal
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class MessageMention : IMessageMention
    {
        private AllowedMention _allowedMention;
        private List<string> _users;
        private List<string> _roles;

        public AllowedMention AllowedMention { get => _allowedMention; set => _allowedMention = value; }

        [JsonProperty(PropertyName = "parse")]
        public List<string> allowedMention
        {
            get
            {
                AllowedMention allowedResult = AllowedMention.NONE;

                // We use our own processing because the user can allow mutual exclusion
                // See https://discord.com/developers/docs/resources/channel#allowed-mentions-object-allowed-mentions-reference

                if (AllowedMention.HasFlag(AllowedMention.USERS) && _users.Count == 0)
                    allowedResult = AllowedMention.USERS;
                if (AllowedMention.HasFlag(AllowedMention.ROLES) && _roles.Count == 0)
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
        public List<string> Users { get => _users; }

        [JsonProperty("roles")]
        public List<string> Roles { get => _roles; }

        public MessageMention()
        {
            _allowedMention = AllowedMention.NONE;
            _users = new List<string>();
            _roles = new List<string>();
        }

        public MessageMention(AllowedMention mention) : this()
        {
            _allowedMention = mention;
        }
    }
}
