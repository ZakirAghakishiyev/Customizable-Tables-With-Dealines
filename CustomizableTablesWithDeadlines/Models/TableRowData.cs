namespace CustomizableTablesWithDeadlines.Models;

public class TableRowData
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Dictionary<Guid, object?> CellValues { get; set; } = new();
    public List<DeadlineItem> Deadlines { get; set; } = [];
}
