using System.Globalization;
using System.Windows.Data;

namespace CustomizableTablesWithDeadlines.Converters;

public class ColumnTypeToTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Models.Enums.ColumnType type)
            return value?.ToString() ?? string.Empty;

        return LocalizationHelper.Strings.GetColumnTypeText(type);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
