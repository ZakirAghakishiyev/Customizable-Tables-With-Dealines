using CustomizableTablesWithDeadlines.Application.DTOs.Cells;
using CustomizableTablesWithDeadlines.Application.DTOs.Deadlines;

namespace CustomizableTablesWithDeadlines.Application.DTOs.Rows;

public class RowDetailsDto
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public int OrderNumber { get; set; }
    public List<CellValueDto> Cells { get; set; } = [];
    public List<DeadlineDto> Deadlines { get; set; } = [];
}
