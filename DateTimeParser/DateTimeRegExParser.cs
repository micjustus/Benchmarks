using System.Text.RegularExpressions;

namespace Benchmarks.DateTimeParser
{
    public partial class DateTimeRegExParser
    {
        [GeneratedRegex(@"^(?<year>\d{4})-(?<month>\d{1,2})-(?<day>\d{1,2})\s+(?<hour>\d{1,2}):(?<minute>\d{1,2}):(?<second>\d{1,2})(?<offset>(?:(?<offsetSign>[+\-])(?<offsetHour>\d{1,2})(?::?(?<offsetMinute>\d{2}))|(?<utc>Z)))?$", RegexOptions.IgnoreCase, "en-GB")]
        private static partial Regex DateTimeFormatWithOffsetRegEx();

        public static DateTime? Parse(string? dateTime, string?[]? formats)
        {
            if (string.IsNullOrEmpty(dateTime)) return null;

            var match = DateTimeFormatWithOffsetRegEx().Match(dateTime);
            if (!match.Success)
                return null;

            int year = int.Parse(match.Groups["year"].Value);
            int month = int.Parse(match.Groups["month"].Value);
            int day = int.Parse(match.Groups["day"].Value);
            int hour = int.Parse(match.Groups["hour"].Value);
            int minute = int.Parse(match.Groups["minute"].Value);
            int second = int.Parse(match.Groups["second"].Value);

            DateTime dt = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);

            // Check for offset
            if (!match.Groups["offset"].Success || match.Groups["utc"].Success)
                return dt;

            int offsetHours = int.Parse(match.Groups["offsetHour"].Value);
            int offsetMinutes = match.Groups["offsetMinute"].Success
                                    ? int.Parse(match.Groups["offsetMinute"].Value)
                                    : 0;
            int sign = match.Groups["offsetSign"].Value == "-" ? -1 : 1;
            TimeSpan offset = new TimeSpan(sign * offsetHours, sign * offsetMinutes, 0);

            dt = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);

            return new DateTimeOffset(dt, offset).UtcDateTime;
        }
    }
}
