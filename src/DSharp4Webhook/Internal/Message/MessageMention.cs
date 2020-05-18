using DSharp4Webhook.Core;
using DSharp4Webhook.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace DSharp4Webhook.Internal
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore, MemberSerialization = MemberSerialization.OptIn)]
    internal sealed class MessageMention : IMessageMention
    {
        public AllowedMention AllowedMention { get; set; }

        [JsonProperty("users")]
        public string[] Users
        {
            get => _users.ToArray();
            set
            {
                _users.Clear();
                if (value != null)
                    _users.AddRange(value);
            }
        }
        [JsonProperty("roles")]
        public string[] Roles
        {
            get => _roles.ToArray();
            set
            {
                _roles.Clear();
                if (value != null)
                    _roles.AddRange(value);
            }
        }

        private List<string> _users;
        private List<string> _roles;

        public MessageMention()
        {
            AllowedMention = AllowedMention.NONE;
            _users = new List<string>();
            _roles = new List<string>();
        }

        public MessageMention(AllowedMention mention) : this()
        {
            AllowedMention = mention;
        }

        public SerializeContext Serialize()
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

            // Note that passing a Falsy value ([], null) into the "users" field does not trigger a validation error
            var jobject = JObject.FromObject(this);
            jobject.AddFirst(new JProperty("parse", new JArray(allowedList)));
            allowedList.Clear();

            // You don't need to use UTF-8, there shouldn't be unicode here
            return new SerializeContext(Encoding.ASCII.GetBytes(jobject.ToString()));
        }
    }
}
