namespace CustomizableTablesWithDeadlines.Application.DTOs.Rows;

public class UpdateCellValueDto
{
    public int RowId { get; set; }
    public int ColumnId { get; set; }
    public string? TextValue { get; set; }
    public decimal? NumberValue { get; set; }
    public DateTime? DateTimeValue { get; set; }
    public bool? BooleanValue { get; set; }
}
