using CustomizableTablesWithDeadlines.Models.Enums;

namespace CustomizableTablesWithDeadlines.Models;

public class TableColumnDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public ColumnType Type { get; set; } = ColumnType.Text;
    public int Order { get; set; }
}
