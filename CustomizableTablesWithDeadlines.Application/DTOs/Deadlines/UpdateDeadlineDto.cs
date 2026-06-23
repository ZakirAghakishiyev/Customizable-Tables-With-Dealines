namespace CustomizableTablesWithDeadlines.Application.DTOs.Deadlines;

public class UpdateDeadlineDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime DeadlineDateTime { get; set; }
}
