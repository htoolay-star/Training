using TimeZoneConverter;

namespace Shared.Extensions
{
    public static class DateTimeHelper
    {

        private static readonly TimeZoneInfo MyanmarTimeZone =
            TZConvert.GetTimeZoneInfo("Asia/Yangon");

        public static DateTime CurrentMyanmarDateTime =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, MyanmarTimeZone);

        public static DateTimeOffset MyanmarNowOffset =>
            TimeZoneInfo.ConvertTime(
                DateTimeOffset.UtcNow,
                MyanmarTimeZone);
    }
}
