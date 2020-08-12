using Newtonsoft.Json.Serialization;

namespace DSharp4Webhook.Util
{
    public sealed class LowerCaseNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name) => name.ToLowerInvariant();
    }
}
