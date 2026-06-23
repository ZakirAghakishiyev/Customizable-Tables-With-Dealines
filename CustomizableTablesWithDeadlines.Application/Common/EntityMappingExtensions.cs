using CustomizableTablesWithDeadlines.Application.DTOs.Cells;
using CustomizableTablesWithDeadlines.Application.DTOs.Columns;
using CustomizableTablesWithDeadlines.Application.DTOs.Deadlines;
using CustomizableTablesWithDeadlines.Application.DTOs.Notifications;
using CustomizableTablesWithDeadlines.Application.DTOs.Rows;
using CustomizableTablesWithDeadlines.Application.DTOs.Tables;
using CustomizableTablesWithDeadlines.Application.DTOs.Settings;
using CustomizableTablesWithDeadlines.Application.Settings;
using CustomizableTablesWithDeadlines.Domain.Entities;

namespace CustomizableTablesWithDeadlines.Application.Common;

public static class EntityMappingExtensions
{
    public static TableDto ToDto(this Table table) => new()
    {
        Id = table.Id,
        Name = table.Name,
        ColumnCount = table.Columns.Count,
        RowCount = table.Rows.Count,
        CreatedAt = table.CreatedAt,
        UpdatedAt = table.UpdatedAt
    };

    public static TableDetailsDto ToDetailsDto(this Table table) => new()
    {
        Id = table.Id,
        Name = table.Name,
        CreatedAt = table.CreatedAt,
        UpdatedAt = table.UpdatedAt,
        Columns = table.Columns.OrderBy(c => c.OrderIndex).Select(c => c.ToDto()).ToList(),
        Rows = table.Rows.OrderBy(r => r.OrderNumber).Select(r => r.ToDetailsDto(table.Columns)).ToList()
    };

    public static ColumnDto ToDto(this Column column) => new()
    {
        Id = column.Id,
        TableId = column.TableId,
        Name = column.Name,
        DataType = column.DataType,
        OrderIndex = column.OrderIndex,
        IsRequired = column.IsRequired
    };

    public static RowDto ToDto(this Row row) => new()
    {
        Id = row.Id,
        TableId = row.TableId,
        OrderNumber = row.OrderNumber
    };

    public static RowDetailsDto ToDetailsDto(this Row row, IEnumerable<Column> columns)
    {
        var columnList = columns.ToList();
        return new RowDetailsDto
        {
            Id = row.Id,
            TableId = row.TableId,
            OrderNumber = row.OrderNumber,
            Cells = columnList
                .Select(column =>
                {
                    var cell = row.CellValues.FirstOrDefault(c => c.ColumnId == column.Id);
                    return cell?.ToDto(column) ?? new CellValueDto
                    {
                        RowId = row.Id,
                        ColumnId = column.Id,
                        ColumnName = column.Name,
                        DataType = column.DataType
                    };
                })
                .ToList(),
            Deadlines = row.Deadlines.Select(d => d.ToDto()).ToList()
        };
    }

    public static CellValueDto ToDto(this CellValue cell, Column column) => new()
    {
        Id = cell.Id,
        RowId = cell.RowId,
        ColumnId = cell.ColumnId,
        ColumnName = column.Name,
        DataType = column.DataType,
        TextValue = cell.ValueText,
        NumberValue = cell.ValueNumber,
        DateTimeValue = cell.ValueDateTime,
        BooleanValue = cell.ValueBoolean
    };

    public static DeadlineDto ToDto(this Deadline deadline) => new()
    {
        Id = deadline.Id,
        RowId = deadline.RowId,
        Title = deadline.Title,
        DeadlineDateTime = deadline.DeadlineDateTime,
        NotificationRules = deadline.NotificationRules?.Select(r => r.ToDto()).ToList() ?? []
    };

    public static NotificationRuleDto ToDto(this NotificationRule rule) => new()
    {
        Id = rule.Id,
        DeadlineId = rule.DeadlineId,
        NotifyBeforeMinutes = rule.NotifyBeforeMinutes,
        IsEnabled = rule.IsEnabled
    };

    public static NotificationLogDto ToDto(this NotificationLog log) => new()
    {
        Id = log.Id,
        DeadlineId = log.DeadlineId,
        NotificationRuleId = log.NotificationRuleId,
        ScheduledFor = log.ScheduledFor,
        SentAt = log.SentAt,
        Status = log.Status
    };

    public static AppSettingsDto ToDto(this AppSettings settings) => new()
    {
        Language = settings.Language,
        DefaultNotifyBeforeMinutes = settings.DefaultNotifyBeforeMinutes,
        EnableDesktopNotifications = settings.EnableDesktopNotifications,
        EnableSound = settings.EnableSound,
        StartWithWindows = settings.StartWithWindows,
        Theme = settings.Theme
    };

    public static AppSettings ToEntity(this AppSettingsDto dto) => new()
    {
        Language = dto.Language,
        DefaultNotifyBeforeMinutes = dto.DefaultNotifyBeforeMinutes,
        EnableDesktopNotifications = dto.EnableDesktopNotifications,
        EnableSound = dto.EnableSound,
        StartWithWindows = dto.StartWithWindows,
        Theme = dto.Theme
    };
}
