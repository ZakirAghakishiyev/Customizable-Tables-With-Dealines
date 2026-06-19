using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Converters;

public static class ColumnTypeValues
{
    public static ColumnType[] All { get; } = Enum.GetValues<ColumnType>();
}
