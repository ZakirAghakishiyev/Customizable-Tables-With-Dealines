using CustomizableTablesWithDeadlines.Application.DTOs.Columns;
using CustomizableTablesWithDeadlines.Application.DTOs.Rows;

namespace CustomizableTablesWithDeadlines.Application.DTOs.Tables;

public class TableDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<ColumnDto> Columns { get; set; } = [];
    public List<RowDetailsDto> Rows { get; set; } = [];
}
