namespace CustomizableTablesWithDeadlines.Application.DTOs.Import;

public class ImportTableResultDto
{
    public string SuggestedTableName { get; set; } = string.Empty;
    public List<string> ColumnNames { get; set; } = [];
    public List<List<string>> Rows { get; set; } = [];
}
