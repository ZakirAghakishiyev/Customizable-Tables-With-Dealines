using CustomizableTablesWithDeadlines.Domain.Entities.Base;
using CustomizableTablesWithDeadlines.Domain.Enums;

namespace CustomizableTablesWithDeadlines.Domain.Entities;

public class Column : AuditedEntity
{
    public int TableId { get; set; }
    public Table Table { get; set; } = null!;

    public string Name { get; set; } = null!;
    public ColumnDataType DataType { get; set; }

    public int OrderIndex { get; set; }
    public bool IsRequired { get; set; }

    public ICollection<CellValue> CellValues { get; set; } = new List<CellValue>();
}
