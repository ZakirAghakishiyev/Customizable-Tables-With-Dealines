namespace CustomizableTablesWithDeadlines.Models;

public class CustomTable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<TableColumnDefinition> Columns { get; set; } = [];
    public List<TableRowData> Rows { get; set; } = [];
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    public int ColumnCount => Columns.Count;
    public int RowCount => Rows.Count;
}
