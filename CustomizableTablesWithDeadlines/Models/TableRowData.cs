namespace CustomizableTablesWithDeadlines.Models;

public class TableRowData
{
    public int Id { get; set; }
    public Dictionary<int, object?> CellValues { get; set; } = new();
    public List<DeadlineItem> Deadlines { get; set; } = [];
}
