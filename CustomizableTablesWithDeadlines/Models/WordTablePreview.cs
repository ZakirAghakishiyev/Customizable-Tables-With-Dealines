namespace CustomizableTablesWithDeadlines.Models;

public class WordTablePreview
{
    public int Index { get; set; }
    public List<List<string>> Rows { get; set; } = [];

    public int RowCount => Rows.Count;
    public int ColumnCount => Rows.FirstOrDefault()?.Count ?? 0;

    public string DisplayName => $"Table {Index + 1} ({RowCount} × {ColumnCount})";
}
