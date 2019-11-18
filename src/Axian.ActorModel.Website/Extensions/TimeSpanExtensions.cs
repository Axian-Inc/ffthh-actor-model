using System;

namespace Axian.ActorModel.Website.Extensions
{
    public static class TimeSpanExtensions
    {
        public static DateTimeOffset FromUnixTimeMilliseconds(this TimeSpan value)
        {
            long ms = Convert.ToInt64(value.TotalMilliseconds);
            return DateTimeOffset.FromUnixTimeMilliseconds(ms);
        }
    }
}
