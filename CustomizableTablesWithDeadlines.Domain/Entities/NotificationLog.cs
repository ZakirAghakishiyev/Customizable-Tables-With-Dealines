using CustomizableTablesWithDeadlines.Domain.Entities.Base;
using CustomizableTablesWithDeadlines.Domain.Enums;

namespace CustomizableTablesWithDeadlines.Domain.Entities;

public class NotificationLog : AuditedEntity
{
    public int DeadlineId { get; set; }
    public Deadline Deadline { get; set; } = null!;

    public int NotificationRuleId { get; set; }
    public NotificationRule NotificationRule { get; set; } = null!;

    public DateTime ScheduledFor { get; set; }
    public DateTime? SentAt { get; set; }

    public NotificationStatus Status { get; set; }
}