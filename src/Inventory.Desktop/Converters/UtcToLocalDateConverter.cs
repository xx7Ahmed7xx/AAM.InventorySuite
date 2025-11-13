using System;
using System.Globalization;
using System.Windows.Data;

namespace Inventory.Desktop.Converters;

/// <summary>
/// Converts UTC DateTime to local time for display
/// </summary>
public class UtcToLocalDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return string.Empty;

        if (value is DateTime dateTime)
        {
            // Ensure the DateTime is treated as UTC
            var utc = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            var local = utc.ToLocalTime();

            // Use format from parameter or default
            // Always use InvariantCulture to prevent Arabic date formatting
            // Default to format with AM/PM: "M/d/yyyy h:mm tt"
            var format = parameter as string ?? "M/d/yyyy h:mm tt";
            return local.ToString(format, CultureInfo.InvariantCulture);
        }

        DateTime? nullableDateTime = (DateTime?)value;
        if (nullableDateTime.HasValue)
        {
            var utc = DateTime.SpecifyKind(nullableDateTime.Value, DateTimeKind.Utc);
            var local = utc.ToLocalTime();

            // Default to format with AM/PM: "M/d/yyyy h:mm tt"
            var format = parameter as string ?? "M/d/yyyy h:mm tt";
            // Always use InvariantCulture to prevent Arabic date formatting
            return local.ToString(format, CultureInfo.InvariantCulture);
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

