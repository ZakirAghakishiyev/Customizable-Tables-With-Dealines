using CustomizableTablesWithDeadlines.Domain.Enums;

namespace CustomizableTablesWithDeadlines.Application.DTOs.Columns;

public class ChangeColumnDataTypeDto
{
    public int Id { get; set; }
    public ColumnDataType DataType { get; set; }
}
