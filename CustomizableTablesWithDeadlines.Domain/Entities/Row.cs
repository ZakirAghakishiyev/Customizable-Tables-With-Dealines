using CustomizableTablesWithDeadlines.Domain.Entities.Base;

namespace CustomizableTablesWithDeadlines.Domain.Entities;

public class Row : AuditedEntity
{
    public int TableId { get; set; }
    public Table Table { get; set; } = null!;

    public int OrderNumber { get; set; }

    public ICollection<CellValue> CellValues { get; set; } = new List<CellValue>();
    public ICollection<Deadline> Deadlines { get; set; } = new List<Deadline>();
}
