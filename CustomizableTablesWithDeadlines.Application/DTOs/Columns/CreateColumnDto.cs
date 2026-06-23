using CustomizableTablesWithDeadlines.Domain.Enums;

namespace CustomizableTablesWithDeadlines.Application.DTOs.Columns;

public class CreateColumnDto
{
    public int TableId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ColumnDataType DataType { get; set; } = ColumnDataType.Text;
    public bool IsRequired { get; set; }
}
