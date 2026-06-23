using CustomizableTablesWithDeadlines.Application.DTOs.Notifications;

namespace CustomizableTablesWithDeadlines.Application.DTOs.Deadlines;

public class DeadlineDto
{
    public int Id { get; set; }
    public int RowId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime DeadlineDateTime { get; set; }
    public List<NotificationRuleDto> NotificationRules { get; set; } = [];
}
