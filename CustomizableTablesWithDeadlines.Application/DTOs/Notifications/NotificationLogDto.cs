using CustomizableTablesWithDeadlines.Domain.Enums;

namespace CustomizableTablesWithDeadlines.Application.DTOs.Notifications;

public class NotificationLogDto
{
    public int Id { get; set; }
    public int DeadlineId { get; set; }
    public int NotificationRuleId { get; set; }
    public DateTime ScheduledFor { get; set; }
    public DateTime? SentAt { get; set; }
    public NotificationStatus Status { get; set; }
}
