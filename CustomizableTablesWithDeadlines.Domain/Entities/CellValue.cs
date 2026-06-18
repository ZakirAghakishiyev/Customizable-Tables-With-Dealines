using CustomizableTablesWithDeadlines.Domain.Entities.Base;

namespace CustomizableTablesWithDeadlines.Domain.Entities;

public class CellValue : AuditedEntity
{
    public int RowId { get; set; }
    public Row Row { get; set; } = null!;

    public int ColumnId { get; set; }
    public Column Column { get; set; } = null!;

    public string? ValueText { get; set; }
    public DateTime? ValueDateTime { get; set; }
    public decimal? ValueNumber { get; set; }
    public bool? ValueBoolean { get; set; }
}
