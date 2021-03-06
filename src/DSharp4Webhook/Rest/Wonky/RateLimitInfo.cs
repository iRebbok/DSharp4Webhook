using DSharp4Webhook.Buffers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace DSharp4Webhook.Rest
{
    /// <summary>
    ///     Information about RateLimit.
    /// </summary>
    /// <remarks>
    ///     Taken from
    ///     https://github.com/discord-net/Discord.Net/blob/ed869bd78b8ae152805b449b759714839b429ce5/src/Discord.Net.Rest/Net/RateLimitInfo.cs
    ///     with some modifications.
    /// </remarks>
    public readonly struct RateLimitInfo
    {
        /// <summary>
        ///     RateLimit is global.
        /// </summary>
        public bool IsGlobal { get; }
        /// <summary>
        ///    Total number of requests that can be made before entering the RateLimit.
        /// </summary>
        public int? Limit { get; }
        /// <summary>
        ///     How many requests can be made before entering the RateLimit.
        /// </summary>
        public int? Remaining { get; }
        /// <summary>
        ///     How long to wait before the Rate Limit expires.
        /// </summary>
        public uint? RetryAfter { get; }
        /// <summary>
        ///     Date when the Rate Limit is reset.
        /// </summary>
        public DateTimeOffset? Reset { get; }
        /// <summary>
        ///     How long wait before can repeat the request.
        /// </summary>
        public TimeSpan? ResetAfter { get; }
        /// <summary>
        ///     Date sent by Discord servers.
        ///     This helps find time lags.
        /// </summary>
        public DateTimeOffset? Date { get; }
        /// <summary>
        ///     How long wait before sending a request.
        /// </summary>
        public TimeSpan MustWait
        {
            get
            {
                if (Date.HasValue && RetryAfter.HasValue)
                {
                    long value = Date.Value.AddMilliseconds((long)Lag + RetryAfter.Value).ToUnixTimeMilliseconds() - DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    if (value > 0L)
                        return TimeSpan.FromMilliseconds(value);
                }
                return TimeSpan.Zero;
            }
        }
        /// <summary>
        ///     Are we in the RateLimit.
        /// </summary>
        public bool IsRatelimited { get => MustWait != TimeSpan.Zero; }
        /// <summary>
        ///     Lag between the request Date and the current time.
        /// </summary>
        public ulong Lag { get; }

        private RateLimitInfo(Dictionary<string, string> headers)
        {
            IsGlobal = headers.TryGetValue("x-ratelimit-global", out string temp) &&
                bool.TryParse(temp, out var isGlobal) && isGlobal;

            Date = headers.TryGetValue("Date", out temp)
                && DateTimeOffset.TryParse(temp, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                ? date : (DateTimeOffset?)null;

            Limit = headers.TryGetValue("x-ratelimit-limit", out temp) &&
                int.TryParse(temp, NumberStyles.None, CultureInfo.InvariantCulture, out var limit)
                ? limit : (int?)null;

            Remaining = headers.TryGetValue("x-ratelimit-remaining", out temp) &&
                int.TryParse(temp, NumberStyles.None, CultureInfo.InvariantCulture, out var remaining)
                ? remaining : (int?)null;

            Reset = headers.TryGetValue("x-ratelimit-reset", out temp) &&
                double.TryParse(temp, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var reset)
                ? DateTimeOffset.FromUnixTimeMilliseconds((long)(reset * 1000L)) : (DateTimeOffset?)null;

            ResetAfter = headers.TryGetValue("x-ratelimit-reset-after", out temp) &&
                float.TryParse(temp, out var resetAfter)
                ? TimeSpan.FromMilliseconds((long)(resetAfter * 1000L)) : (TimeSpan?)null;

            RetryAfter = headers.TryGetValue("retry-after", out temp) &&
                    uint.TryParse(temp, NumberStyles.None, CultureInfo.InvariantCulture, out var retryAfter)
                    ? retryAfter : (uint?)null;

            if (Date.HasValue)
            {
                TimeSpan lag = DateTimeOffset.UtcNow - Date.Value;
                if (lag.TotalMilliseconds > 0D)
                    Lag = Convert.ToUInt64(Math.Ceiling(lag.TotalMilliseconds));
                else
                    Lag = 0L;
            }
            else
            {
                Lag = 0L;
            }
        }

        public static bool operator ==(RateLimitInfo left, RateLimitInfo right)
        {
            return left.IsGlobal == right.IsGlobal &&
                left.Limit == right.Limit &&
                left.Remaining == right.Remaining &&
                left.RetryAfter == left.RetryAfter &&
                left.ResetAfter == right.ResetAfter &&
                left.Reset == right.Reset &&
                left.Date == right.Date;
        }

        public static bool operator !=(RateLimitInfo left, RateLimitInfo right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj) => obj is RateLimitInfo info && info == this;

        public override int GetHashCode() => base.GetHashCode();

        public static RateLimitInfo Get(WebHeaderCollection collection)
        {
            var dict = DictionaryStringPool.Rent();
            foreach (var headerKey in collection.AllKeys)
                dict.Add(headerKey, collection.Get(headerKey));

            var ratelimit = new RateLimitInfo(dict);
            DictionaryStringPool.Recycle(dict);
            return ratelimit;
        }

        public static RateLimitInfo Get(HttpResponseHeaders responseHeaders)
        {
            var dict = DictionaryStringPool.Rent();
            foreach (var pair in responseHeaders)
            {
                var value = pair.Value.SingleOrDefault();
                if (value == null)
                    continue;

                dict.Add(pair.Key, value);
            }

            var ratelimit = new RateLimitInfo(dict);
            DictionaryStringPool.Recycle(dict);
            return ratelimit;
        }
    }
}
