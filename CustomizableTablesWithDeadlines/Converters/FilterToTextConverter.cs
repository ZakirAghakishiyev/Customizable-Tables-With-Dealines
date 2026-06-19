using System.Globalization;
using System.Windows.Data;
using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Converters;

public class FilterToTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DeadlineFilter filter)
            return value?.ToString() ?? string.Empty;

        var strings = LocalizationHelper.Strings;
        return filter switch
        {
            DeadlineFilter.Today => strings.FilterToday,
            DeadlineFilter.ThisWeek => strings.FilterThisWeek,
            DeadlineFilter.Overdue => strings.FilterOverdue,
            _ => strings.FilterAll
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
