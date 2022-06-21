using System;

public static class DateTimeHelper
{
    public static DateTime ToMexicoTime(this DateTime d)
    {
        return TimeZoneInfo.ConvertTime(d,
           TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time (Mexico)"));
    }
}

