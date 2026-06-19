using System.Globalization;
using System.Windows.Data;
using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Converters;

public class NotifyOptionToTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not NotifyBeforeOption option)
            return value?.ToString() ?? string.Empty;

        return LocalizationHelper.Strings.GetNotifyBeforeText(option);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
