using CustomizableTablesWithDeadlines.Application.DTOs.Cells;
using CustomizableTablesWithDeadlines.Application.DTOs.Columns;
using CustomizableTablesWithDeadlines.Application.DTOs.Deadlines;
using CustomizableTablesWithDeadlines.Application.DTOs.Rows;
using CustomizableTablesWithDeadlines.Application.DTOs.Tables;
using CustomizableTablesWithDeadlines.Domain.Enums;
using CustomizableTablesWithDeadlines.Models;
using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Mapping;

public static class PresentationMapping
{
    public static CustomTable ToCustomTable(TableDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        LastUpdated = dto.UpdatedAt ?? dto.CreatedAt,
        ListedColumnCount = dto.ColumnCount,
        ListedRowCount = dto.RowCount
    };

    public static CustomTable ToCustomTable(TableDetailsDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        LastUpdated = dto.UpdatedAt ?? dto.CreatedAt,
        Columns = dto.Columns.Select(ToColumnDefinition).ToList(),
        Rows = dto.Rows.Select(ToRowData).ToList()
    };

    public static TableColumnDefinition ToColumnDefinition(ColumnDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Type = ToColumnType(dto.DataType),
        Order = dto.OrderIndex
    };

    public static TableRowData ToRowData(RowDetailsDto dto) => new()
    {
        Id = dto.Id,
        CellValues = dto.Cells.ToDictionary(c => c.ColumnId, c => ToCellObject(c)),
        Deadlines = dto.Deadlines.Select(ToDeadlineItem).ToList()
    };

    public static object? ToCellObject(CellValueDto cell) => cell.DataType switch
    {
        ColumnDataType.Number => cell.NumberValue,
        ColumnDataType.DateTime => cell.DateTimeValue,
        ColumnDataType.Boolean => cell.BooleanValue,
        _ => cell.TextValue
    };

    public static DeadlineItem ToDeadlineItem(DeadlineDto dto) => new()
    {
        Id = dto.Id,
        Title = dto.Title,
        DeadlineDateTime = dto.DeadlineDateTime,
        NotifyBefore = dto.NotificationRules.FirstOrDefault() is { } rule
            ? MinutesToNotifyOption(rule.NotifyBeforeMinutes)
            : NotifyBeforeOption.OneHour
    };

    public static ColumnType ToColumnType(ColumnDataType dataType) => dataType switch
    {
        ColumnDataType.Number => ColumnType.Number,
        ColumnDataType.DateTime => ColumnType.DateTime,
        ColumnDataType.Boolean => ColumnType.Boolean,
        _ => ColumnType.Text
    };

    public static ColumnDataType ToColumnDataType(ColumnType type) => type switch
    {
        ColumnType.Number => ColumnDataType.Number,
        ColumnType.DateTime => ColumnDataType.DateTime,
        ColumnType.Boolean => ColumnDataType.Boolean,
        _ => ColumnDataType.Text
    };

    public static NotifyBeforeOption MinutesToNotifyOption(int minutes) => minutes switch
    {
        <= 10 => NotifyBeforeOption.TenMinutes,
        <= 30 => NotifyBeforeOption.ThirtyMinutes,
        <= 60 => NotifyBeforeOption.OneHour,
        <= 180 => NotifyBeforeOption.ThreeHours,
        <= 1440 => NotifyBeforeOption.OneDay,
        _ => NotifyBeforeOption.Custom
    };

    public static int NotifyOptionToMinutes(NotifyBeforeOption option, int? customMinutes) => option switch
    {
        NotifyBeforeOption.TenMinutes => 10,
        NotifyBeforeOption.ThirtyMinutes => 30,
        NotifyBeforeOption.OneHour => 60,
        NotifyBeforeOption.ThreeHours => 180,
        NotifyBeforeOption.OneDay => 1440,
        NotifyBeforeOption.Custom => customMinutes ?? 30,
        _ => 60
    };
}
