using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Inventory.Desktop.Converters;

public class IntToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Show pagination when TotalCount > PageSize (meaning there's more data than fits on one page)
        // Parameter should be a MultiBinding with TotalCount and PageSize
        // For now, we'll use a different approach - bind to TotalCount and pass PageSize as parameter
        if (value is int totalCount && parameter is int pageSize)
        {
            return totalCount > pageSize ? Visibility.Visible : Visibility.Collapsed;
        }
        // Fallback: if only TotalPages is provided, show when > 1
        if (value is int intValue)
        {
            return intValue > 1 ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

