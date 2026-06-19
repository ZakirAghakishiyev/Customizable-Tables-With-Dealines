using System.Globalization;
using System.Windows.Data;
using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Converters;

public class DeadlineStatusToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not DeadlineStatus status)
            return "#64748B";

        return status switch
        {
            DeadlineStatus.Upcoming => "#3B82F6",
            DeadlineStatus.DueSoon => "#F59E0B",
            DeadlineStatus.Overdue => "#EF4444",
            DeadlineStatus.Completed => "#10B981",
            _ => "#64748B"
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
