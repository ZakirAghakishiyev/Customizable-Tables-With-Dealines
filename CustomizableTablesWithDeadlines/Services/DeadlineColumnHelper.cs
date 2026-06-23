using System.Globalization;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Services;

public static class DeadlineColumnHelper
{
    private static readonly string[] DeadlineColumnKeywords =
    [
        "deadline", "dealine", "dealines", "due", "expires", "expiry", "exp date", "exp."
    ];

    public static bool LooksLikeDeadlineColumn(string columnName)
    {
        var normalized = columnName.Trim().ToLowerInvariant();
        return DeadlineColumnKeywords.Any(keyword => normalized.Contains(keyword));
    }

    public static IEnumerable<TableColumnDefinition> GetDeadlineColumns(IEnumerable<TableColumnDefinition> columns) =>
        columns.Where(c =>
            (c.Type is ColumnType.DateTime or ColumnType.Text) && LooksLikeDeadlineColumn(c.Name));

    public static bool HasDeadlineColumn(IEnumerable<TableColumnDefinition> columns) =>
        GetDeadlineColumns(columns).Any();

    public static DateTime? GetNearestDeadlineDateTime(TableRowData row) =>
        row.Deadlines.Count == 0
            ? null
            : row.Deadlines.MinBy(d => d.DeadlineDateTime)?.DeadlineDateTime;

    public static void SyncRowDeadlinesToColumns(TableRowData row, IEnumerable<TableColumnDefinition> deadlineColumns)
    {
        var columns = deadlineColumns.ToList();
        if (columns.Count == 0)
            return;

        var deadlineDate = GetNearestDeadlineDateTime(row);
        if (deadlineDate is null)
            return;

        foreach (var column in columns)
        {
            row.CellValues[column.Id] = column.Type switch
            {
                ColumnType.DateTime => deadlineDate,
                ColumnType.Text => deadlineDate.Value.ToString("g", CultureInfo.CurrentCulture),
                _ => row.CellValues.GetValueOrDefault(column.Id)
            };
        }
    }
}
