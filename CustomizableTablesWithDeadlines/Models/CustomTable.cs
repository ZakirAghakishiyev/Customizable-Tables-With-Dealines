namespace CustomizableTablesWithDeadlines.Models;

public class CustomTable
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<TableColumnDefinition> Columns { get; set; } = [];
    public List<TableRowData> Rows { get; set; } = [];
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    public int ColumnCount => Columns.Count > 0 ? Columns.Count : ListedColumnCount;
    public int RowCount => Rows.Count > 0 ? Rows.Count : ListedRowCount;
    public int ListedColumnCount { get; set; }
    public int ListedRowCount { get; set; }
}
