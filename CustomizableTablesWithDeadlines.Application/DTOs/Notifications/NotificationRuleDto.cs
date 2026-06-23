namespace CustomizableTablesWithDeadlines.Application.DTOs.Notifications;

public class NotificationRuleDto
{
    public int Id { get; set; }
    public int DeadlineId { get; set; }
    public int NotifyBeforeMinutes { get; set; }
    public bool IsEnabled { get; set; }
}
