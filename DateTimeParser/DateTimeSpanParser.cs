using System.Runtime.CompilerServices;

namespace Benchmarks.DateTimeParser
{
    public class DateTimeSpanParser
    {
        public static DateTimeOffset? ParseDateTimeSpan(ReadOnlySpan<char> input)
        {
            if (input.Length < 13)
                return null;

            // Year is 4 digits followed by '-'
            if (!ParseDigits(input[..4], out int year) || input[4] != '-')
                return null;

            int pos = 5;

            int month, day, hour, minute, second;
            if (!ParseNumber(input, ref pos, '-', out month) ||
                !ParseNumber(input, ref pos, ' ', out day) ||
                !ParseNumber(input, ref pos, ':', out hour) ||
                !ParseNumber(input, ref pos, ':', out minute) ||
                !ParseNumber(input, ref pos, '\0', out second))
                return null;

            TimeSpan offset = TimeSpan.Zero;
            bool hasOffset = false;

            // parse optional timezone offset
            if (pos < input.Length)
            {
                char sign = input[pos];

                if (sign == 'Z')
                {
                    hasOffset = true;
                    pos++; // Skip 'Z'
                }
                else if (sign == '+' || sign == '-')
                {
                    hasOffset = true;
                    pos++; // Skip '+' or '-'

                    // Parse offset hours 
                    if (!ParseNumber(input, ref pos, ':', out int offHour))
                        return null;

                    // Optional offset minutes
                    int offMinute = 0;
                    if (pos < input.Length && input[pos] == ':')
                    {
                        pos++; // Skip ':'
                        if (!ParseNumber(input, ref pos, '\0', out offMinute))
                            return null;
                    }

                    offset = new TimeSpan(offHour, offMinute, 0);
                    if (sign == '-')
                        offset = -offset;
                }
            }

            try
            {
                DateTime dt = new(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
                return hasOffset
                    ? new DateTimeOffset(dt, offset)
                    : new DateTimeOffset(DateTime.SpecifyKind(dt, DateTimeKind.Utc));
            }
            catch
            {
                return null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ParseNumber(ReadOnlySpan<char> input, ref int pos, char separator, out int value)
        {
            int start = pos;
            while (pos < input.Length && IsDigit(input[pos])) pos++;

            if (start == pos || (separator != '\0' && (pos >= input.Length || input[pos] != separator)))
            {
                value = 0;
                return false;
            }

            if (!ParseDigits(input[start..pos], out value))
                return false;

            if (separator != '\0')
                pos++; 

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ParseDigits(ReadOnlySpan<char> input, out int result)
        {
            result = 0;
            if (input.Length == 0)
                return false;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (!IsDigit(c))
                    return false;

                result = result * 10 + (c - '0');
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsDigit(char c) => c >= '0' && c <= '9';

        public static DateTimeOffset? Parse(string? input) =>
            input is null ? null : ParseDateTimeSpan(input.AsSpan());


        //public static DateTimeOffset? ParseDateTimeSpan(ReadOnlySpan<char> input)
        //{
        //    // "YYYY-M-D H:m:s"
        //    if (input.Length < 13)
        //        return null;

        //    if (!int.TryParse(input[..4], out int year) ||  input[4] != '-') return null;

        //    // Parse month 
        //    int pos = 5;
        //    int monthStart = pos;
        //    while (pos < input.Length && input[pos] != '-') pos++;
        //    if (pos == input.Length || !int.TryParse(input[monthStart..pos], out int month)) return null;
        //    pos++; // Skip '-'

        //    // Parse day
        //    int dayStart = pos;
        //    while (pos < input.Length && input[pos] != ' ') pos++;
        //    if (pos == input.Length || !int.TryParse(input[dayStart..pos], out int day)) return null;
        //    pos++; // Skip ' '

        //    // Parse hour 
        //    int hourStart = pos;
        //    while (pos < input.Length && input[pos] != ':') pos++;
        //    if (pos == input.Length || !int.TryParse(input[hourStart..pos], out int hour)) return null;
        //    pos++; // Skip ':'

        //    // Parse minute 
        //    int minuteStart = pos;
        //    while (pos < input.Length && input[pos] != ':') pos++;
        //    if (pos == input.Length || !int.TryParse(input[minuteStart..pos], out int minute)) return null;
        //    pos++; // Skip ':'

        //    // parse seconds
        //    int secondStart = pos;
        //    while (pos < input.Length && input[pos] != '+' && input[pos] != '-' && input[pos] != 'Z') pos++;
        //    if (!int.TryParse(input[secondStart..pos], out int second)) return null;

        //    TimeSpan offset = TimeSpan.Zero;
        //    bool hasOffset = false;

        //    // Parse timezone offset 
        //    if (pos < input.Length)
        //    {
        //        // do we have 'Z' (UTC)
        //        if (input[pos] == 'Z')
        //        {
        //            hasOffset = true;
        //        }
        //        else if (input[pos] == '+' || input[pos] == '-')
        //        {
        //            hasOffset = true;
        //            bool isNegative = input[pos] == '-';
        //            pos++;

        //            // Parse offset hours
        //            int offHourStart = pos;
        //            while (pos < input.Length && char.IsDigit(input[pos])) pos++;
        //            if (offHourStart == pos || !int.TryParse(input[offHourStart..pos], out int offHour)) return null;

        //            // can have an optional colon and minutes
        //            int offMinute = 0;
        //            if (pos < input.Length && input[pos] == ':')
        //            {
        //                pos++; // Skip ':'
        //                int offMinStart = pos;
        //                while (pos < input.Length && char.IsDigit(input[pos])) pos++;
        //                if (offMinStart == pos || !int.TryParse(input[offMinStart..pos], out offMinute)) return null;
        //            }

        //            offset = new TimeSpan(offHour, offMinute, 0);
        //            if (isNegative)
        //                offset = -offset;
        //        }
        //    }

        //    DateTime dt;
        //    try
        //    {
        //        dt = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);

        //        return hasOffset
        //            ? new DateTimeOffset(dt, offset)
        //            : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

    }

}
