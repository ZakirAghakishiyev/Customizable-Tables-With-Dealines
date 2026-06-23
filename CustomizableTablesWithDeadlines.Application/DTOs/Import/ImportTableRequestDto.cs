namespace CustomizableTablesWithDeadlines.Application.DTOs.Import;

public class ImportTableRequestDto
{
    public string FilePath { get; set; } = string.Empty;
    public int TableIndex { get; set; }
    public bool FirstRowAsHeader { get; set; } = true;
    public string? TableName { get; set; }
    public bool CreateDeadlinesFromDateColumns { get; set; }
    public List<int> DateTimeColumnIndices { get; set; } = [];
}
