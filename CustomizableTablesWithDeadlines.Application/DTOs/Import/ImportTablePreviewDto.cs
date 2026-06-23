namespace CustomizableTablesWithDeadlines.Application.DTOs.Import;

public class ImportTablePreviewDto
{
    public int TableIndex { get; set; }
    public int RowCount { get; set; }
    public int ColumnCount { get; set; }
    public List<List<string>> PreviewRows { get; set; } = [];
}
