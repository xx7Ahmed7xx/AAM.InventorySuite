using System;
using System.Globalization;

namespace Inventory.Desktop.Utils;

/// <summary>
/// Utility class for date/time handling and localization
/// </summary>
public static class DateUtils
{
    /// <summary>
    /// Converts a UTC DateTime to local time and formats it according to the current culture
    /// </summary>
    public static string FormatLocalDateTime(DateTime? utcDateTime, string format = "g")
    {
        if (!utcDateTime.HasValue)
            return string.Empty;

        // Ensure the DateTime is treated as UTC
        var utc = DateTime.SpecifyKind(utcDateTime.Value, DateTimeKind.Utc);
        var local = utc.ToLocalTime();

        return local.ToString(format, CultureInfo.CurrentCulture);
    }

    /// <summary>
    /// Converts a UTC DateTime string to local time and formats it
    /// </summary>
    public static string FormatLocalDateTime(string utcDateTimeString, string format = "g")
    {
        if (string.IsNullOrEmpty(utcDateTimeString))
            return string.Empty;

        if (DateTime.TryParse(utcDateTimeString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var dateTime))
        {
            return FormatLocalDateTime(dateTime, format);
        }

        return utcDateTimeString;
    }

    /// <summary>
    /// Gets a short date/time format string for the current culture
    /// </summary>
    public static string GetShortDateTimeFormat()
    {
        return CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + 
               CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern;
    }
}

