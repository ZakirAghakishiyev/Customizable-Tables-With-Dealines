using CustomizableTablesWithDeadlines.Domain.Enums;

namespace CustomizableTablesWithDeadlines.Application.DTOs.Columns;

public class ColumnDto
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ColumnDataType DataType { get; set; }
    public int OrderIndex { get; set; }
    public bool IsRequired { get; set; }
}
