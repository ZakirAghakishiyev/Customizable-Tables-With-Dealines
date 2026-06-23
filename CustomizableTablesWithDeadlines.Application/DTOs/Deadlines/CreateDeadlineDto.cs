namespace CustomizableTablesWithDeadlines.Application.DTOs.Deadlines;

public class CreateDeadlineDto
{
    public int RowId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime DeadlineDateTime { get; set; }
}
