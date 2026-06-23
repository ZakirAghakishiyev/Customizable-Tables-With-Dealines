namespace CustomizableTablesWithDeadlines.Application.DTOs.Notifications;

public class CreateNotificationRuleDto
{
    public int DeadlineId { get; set; }
    public int NotifyBeforeMinutes { get; set; }
    public bool IsEnabled { get; set; } = true;
}
