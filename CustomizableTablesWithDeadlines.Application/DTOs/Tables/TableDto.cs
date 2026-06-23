namespace CustomizableTablesWithDeadlines.Application.DTOs.Tables;

public class TableDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ColumnCount { get; set; }
    public int RowCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
