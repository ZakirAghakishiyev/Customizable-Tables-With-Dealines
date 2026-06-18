using CustomizableTablesWithDeadlines.Domain.Entities.Base;

namespace CustomizableTablesWithDeadlines.Domain.Entities;

public class NotificationRule : AuditedEntity
{
    public int DeadlineId { get; set; }
    public Deadline Deadline { get; set; } = null!;

    public int NotifyBeforeMinutes { get; set; }
    public bool IsEnabled { get; set; } = true;

    public ICollection<NotificationLog> NotificationLogs { get; set; } = new List<NotificationLog>();
}
