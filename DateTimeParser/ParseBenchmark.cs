using BenchmarkDotNet.Attributes;
using System.Globalization;

namespace Benchmarks.DateTimeParser
{
    public partial class DateTimeParseBenchmarks
    {
        const string WithoutOffset = "yyyy-M-d H:m:s";
        const string With1DigitOffset = "yyyy-M-d H:m:sz";
        const string With2DigitOffset = "yyyy-M-d H:m:szz";
        const string WithKOffset = "yyyy-M-d H:m:sK";
        const string WithHoursMinutesOffset = "yyyy-M-d H:m:szzz";

        public static readonly string?[] formats = new[] { WithoutOffset, With1DigitOffset, With2DigitOffset, WithHoursMinutesOffset, WithKOffset };

        public static readonly string test1 = "2025-02-25 09:00:09+01:00";
        public static readonly string test2 = "2025-02-25 09:00:09+01";
        public static readonly string test3 = "2025-02-25 09:00:09+1";
        public static readonly string test4 = "2025-02-25 09:00:09";
        public static readonly string test5 = "2025-2-25 9:0:9+01:00";
        public static readonly string test6 = "2025-2-25 9:0:9+01";
        public static readonly string test7 = "2025-2-25 9:0:9+1";
        public static readonly string test8 = "2025-2-25 9:0:9";

        [Benchmark]
        public void Parse_WithTryParse()
        {
            _ = ParseNullableDateTime_Parse(test1, formats);
            _ = ParseNullableDateTime_Parse(test2, formats);
            _ = ParseNullableDateTime_Parse(test3, formats);
            _ = ParseNullableDateTime_Parse(test4, formats);
            _ = ParseNullableDateTime_Parse(test5, formats);
            _ = ParseNullableDateTime_Parse(test6, formats);
            _ = ParseNullableDateTime_Parse(test7, formats);
            _ = ParseNullableDateTime_Parse(test8, formats);
        }

        [Benchmark]
        public void Parse_WithTryParseExact()
        {
            _ = ParseNullableDateTime_ParseExact(test1, formats);
            _ = ParseNullableDateTime_ParseExact(test2, formats);
            _ = ParseNullableDateTime_ParseExact(test3, formats);
            _ = ParseNullableDateTime_ParseExact(test4, formats);
            _ = ParseNullableDateTime_ParseExact(test5, formats);
            _ = ParseNullableDateTime_ParseExact(test6, formats);
            _ = ParseNullableDateTime_ParseExact(test7, formats);
            _ = ParseNullableDateTime_ParseExact(test8, formats);
        }

        [Benchmark]
        public void Parse_WithParseRegEx()
        {
            _ = DateTimeRegExParser.Parse(test1, formats);
            _ = DateTimeRegExParser.Parse(test2, formats);
            _ = DateTimeRegExParser.Parse(test3, formats);
            _ = DateTimeRegExParser.Parse(test4, formats);
            _ = DateTimeRegExParser.Parse(test5, formats);
            _ = DateTimeRegExParser.Parse(test6, formats);
            _ = DateTimeRegExParser.Parse(test7, formats);
            _ = DateTimeRegExParser.Parse(test8, formats);
        }

        [Benchmark]
        public void Parse_WithCustomParser()
        {
            _ = DateTimeSpanParser.Parse(test1);
            _ = DateTimeSpanParser.Parse(test2);
            _ = DateTimeSpanParser.Parse(test3);
            _ = DateTimeSpanParser.Parse(test4);
            _ = DateTimeSpanParser.Parse(test5);
            _ = DateTimeSpanParser.Parse(test6);
            _ = DateTimeSpanParser.Parse(test7);
            _ = DateTimeSpanParser.Parse(test8);
        }

        public static DateTime? ParseNullableDateTime_Parse(string? dateTime, string?[]? formats) =>
            string.IsNullOrEmpty(dateTime)
                ? null
                : DateTimeOffset.TryParse(dateTime, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dto)
                    ? dto.UtcDateTime : null;

        public static DateTime? ParseNullableDateTime_ParseExact(string? dateTime, string?[]? formats) =>
            string.IsNullOrEmpty(dateTime)
                ? null
                : DateTimeOffset.TryParseExact(dateTime, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dto)
                    ? dto.UtcDateTime : null;
    }
}