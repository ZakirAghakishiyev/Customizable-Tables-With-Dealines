using CustomizableTablesWithDeadlines.Domain.Entities.Base;

namespace CustomizableTablesWithDeadlines.Domain.Entities;

public class Table : AuditedEntity
{
    public string Name { get; set; } = null!;

    public ICollection<Column> Columns { get; set; } = new List<Column>();
    public ICollection<Row> Rows { get; set; } = new List<Row>();
}
