using CustomizableTablesWithDeadlines.Domain.Enums;

namespace CustomizableTablesWithDeadlines.Application.DTOs.Cells;

public class CellValueDto
{
    public int Id { get; set; }
    public int RowId { get; set; }
    public int ColumnId { get; set; }
    public string? ColumnName { get; set; }
    public ColumnDataType DataType { get; set; }
    public string? TextValue { get; set; }
    public decimal? NumberValue { get; set; }
    public DateTime? DateTimeValue { get; set; }
    public bool? BooleanValue { get; set; }
}
