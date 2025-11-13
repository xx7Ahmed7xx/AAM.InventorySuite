using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Inventory.Desktop.Converters;

/// <summary>
/// Converter that shows visibility when TotalCount > PageSize
/// </summary>
public class CountToVisibilityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values == null || values.Length < 2)
            return Visibility.Collapsed;

        if (values[0] is int totalCount && values[1] is int pageSize)
        {
            // Show pagination when there's any data (totalCount > 0) to allow page size changes
            return totalCount > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        return Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

